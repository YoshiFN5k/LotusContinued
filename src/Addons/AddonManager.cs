using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Lotus.RPC;
using Lotus.Extensions;
using Lotus.Roles;
using VentLib;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using Lotus.API.Player;

namespace Lotus.Addons;

public class AddonManager
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(AddonManager));

    public static LogLevel AddonLL = LogLevel.Info.Similar("ADDON", ConsoleColor.Magenta);
    private static List<LotusAddon> Addons = new();

    public static IEnumerable<LotusAddon> GetAllAddons(bool includeHidden = true) => Addons.Where(x => includeHidden | x.DisableRPC());

    internal static void ImportAddons()
    {
        DirectoryInfo addonDirectory = new("./addons/");
        if (!addonDirectory.Exists)
            addonDirectory.Create();
        addonDirectory.EnumerateFiles().Do(LoadAddon);
        Addons.ForEach(addon =>
        {
            log.Log(AddonLL, $"Calling Post Initialize for {addon.Name}");
            addon.PostInitialize(new List<LotusAddon>(Addons));
            //addon.Factions.Do(f => FactionConstraintValidator.ValidateAndAdd(f, file.Name));
            ProjectLotus.GameModeManager.GameModes.AddRange(addon.Gamemodes);
        });
    }

    private static void LoadAddon(FileInfo file)
    {
        try
        {
            Assembly assembly = Assembly.LoadFile(file.FullName);
            Type? lotusType = assembly.GetTypes().FirstOrDefault(t => t.IsAssignableTo(typeof(LotusAddon)));
            if (lotusType == null)
                throw new ConstraintException($"Lotus Addons requires ONE class file that extends {nameof(LotusAddon)}");
            LotusAddon addon = (LotusAddon)AccessTools.Constructor(lotusType).Invoke(Array.Empty<object>());

            log.Log(AddonLL, $"Loading Addon [{addon.Name} {addon.Version}]", "AddonManager");
            Vents.Register(assembly);

            Addons.Add(addon);
            addon.Initialize();
        }
        catch (Exception e)
        {
            log.Exception($"Error occured while loading addon. Addon File Name: {file.Name}", e);
            log.Exception(e);
        }
    }

    internal static void SendAddonsToHost()
    {
        if (AmongUsClient.Instance.AmHost) return;
        List<AddonInfo> addonsToSend = GetAllAddons(false).Select(AddonInfo.From).ToList();
        Vents.FindRPC((uint)ModCalls.RecieveAddons)!.Send([-1], addonsToSend);
    }

    [ModRPC((uint)ModCalls.RecieveAddons, RpcActors.NonHosts, RpcActors.Host)]
    public static void VerifyClientAddons(List<AddonInfo> receivedAddons)
    {
        List<AddonInfo> hostInfo = Addons.Select(AddonInfo.From).ToList();
        int senderId = Vents.GetLastSender((uint)ModCalls.RecieveAddons)?.GetClientId() ?? 999;
        log.Debug($"Last Sender: {senderId}");


        List<AddonInfo> mismatchInfo = Addons.Select(hostAddon =>
        {
            AddonInfo haInfo = hostInfo.First(h => h.Name == hostAddon.Name);
            AddonInfo? matchingAddon = receivedAddons.FirstOrDefault(a => a == haInfo);
            if (matchingAddon == null)
            {
                haInfo.Mismatches = Mismatch.ClientMissingAddon;
                Vents.BlockClient(hostAddon.BundledAssembly, senderId);
                return haInfo;
            }

            matchingAddon.CheckVersion(matchingAddon);
            return matchingAddon;
        }).ToList();

        mismatchInfo.AddRange(receivedAddons.Select(clientAddon =>
            {
                AddonInfo? matchingAddon = hostInfo.FirstOrDefault(a => a == clientAddon);
                if (matchingAddon! == null!)
                    clientAddon.Mismatches = Mismatch.HostMissingAddon;
                else
                    clientAddon.CheckVersion(matchingAddon);
                return clientAddon;
            }));

        mismatchInfo.DistinctBy(addon => addon.Name).Where(addon => addon.Mismatches is not (Mismatch.None or Mismatch.ClientMissingAddon)).Do(a => Vents.FindRPC(1017)!.Send([senderId], a.AssemblyFullName, 0));
        ReceiveAddonVerification(mismatchInfo.DistinctBy(addon => addon.Name).Where(addon => addon.Mismatches is not Mismatch.None).ToList(), senderId);
    }

    [ModRPC((uint)ModCalls.RecieveAddons, RpcActors.Host, RpcActors.NonHosts, MethodInvocation.ExecuteAfter)]
    public static void ReceiveAddonVerification(List<AddonInfo> addons, int senderId)
    {
        if (addons.Count == 0) return;
        PlayerControl? senderControl = Players.GetAllPlayers().FirstOrDefault(p => p.GetClientId() == senderId);
        string clientName = senderControl == null ? "(could not find sender)" : (senderControl.PlayerId == PlayerControl.LocalPlayer.PlayerId ? "this client" : senderControl.name);
        log.Exception($"VerifyAddons - Error Validating Addons. All CustomRPCs between the host and {clientName} have been disabled.");
        log.Exception(" -=-=-=-=-=-=-=-=-=[Errored Addons]=-=-=-=-=-=-=-=-=-");
        foreach (var rejectReason in addons.Where(info => info.Mismatches is not Mismatch.None).Select(addonInfo => addonInfo.Mismatches
             switch
        {
            Mismatch.Version => $" {addonInfo.Name}:{addonInfo.Version} => Local version is not compatible with the host version of the addon",
            Mismatch.ClientMissingAddon => $" {addonInfo.Name}:{addonInfo.Version} => Client Missing Addon ",
            Mismatch.HostMissingAddon => $" {addonInfo.Name}:{addonInfo.Version} => Host Missing Addon ",
            _ => throw new ArgumentOutOfRangeException()
        }))
            log.Exception("VerifyAddons" + rejectReason);
        if (clientName != "this client") return;
        // stop rpcs from bad addons
    }
}
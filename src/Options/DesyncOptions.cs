using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using Lotus.API;
using Lotus.API.Player;
using Lotus.Roles.Overrides;
using Lotus.Extensions;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;
using InnerNet;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace Lotus.Options;

public static class DesyncOptions
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(DesyncOptions));

    public static void SyncToAll(IGameOptions options) => Players.GetPlayers().Do(p => SyncToPlayer(options, p));

    public static void SyncToPlayer(IGameOptions options, PlayerControl player)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (player == null) return;
        if (!player.AmOwner)
        {
            try
            {
                SyncToClient(options, player.GetClientId());
            }
            catch (Exception exception)
            {
                log.Exception("Error syncing game options to client.", exception);
            }
            return;
        }

        foreach (var com in GameManager.Instance.LogicComponents)
        {
            if (com.TryCast<LogicOptions>(out var lo))
                lo!.SetGameOptions(options);
        }
        GameOptionsManager.Instance.CurrentGameOptions = options;
    }

    public static void SyncToClient(IGameOptions options, int clientId)
    {
        GameOptionsFactory optionsFactory = GameOptionsManager.Instance.gameOptionsFactory;

        // this was taken from town of host..
        MessageWriter writer = MessageWriter.Get(SendOption.None);
        writer.Write(options.Version);
        writer.StartMessage(0);
        writer.Write((byte)options.GameMode);
        if (options.TryCast<NormalGameOptionsV08>(out var normalOpt))
            NormalGameOptionsV08.Serialize(writer, normalOpt);
        else if (options.TryCast<HideNSeekGameOptionsV08>(out var hnsOpt))
            HideNSeekGameOptionsV08.Serialize(writer, hnsOpt);
        else
        {
            writer.Recycle();
            log.Fatal("Option cast failed.");
        }
        writer.EndMessage();

        // 配列化&送信
        var byteArray = new Il2CppStructArray<byte>(writer.Length - 1);
        // MessageWriter.ToByteArray
        Il2CppSystem.Buffer.BlockCopy(writer.Buffer.Cast<Il2CppSystem.Array>(), 1, byteArray.Cast<Il2CppSystem.Array>(), 0, writer.Length - 1);

        GameManager.Instance.LogicComponents.ToArray().ForEach((lc, i) =>
        {
            if (!lc.TryCast<LogicOptions>(out _)) return;
            // the actual writer that sends the game options out
            var realWriter = MessageWriter.Get(SendOption.Reliable);

            realWriter.StartMessage(clientId == -1 ? Tags.GameData : Tags.GameDataTo);
            {
                realWriter.Write(AmongUsClient.Instance.GameId);
                if (clientId != -1) realWriter.WritePacked(clientId);
                realWriter.StartMessage(1);
                {
                    realWriter.WritePacked(GameManager.Instance.NetId);
                    realWriter.StartMessage((byte)i);
                    {
                        realWriter.WriteBytesAndSize(byteArray);
                    }
                    realWriter.EndMessage();
                }
                realWriter.EndMessage();
            }
            realWriter.EndMessage();

            AmongUsClient.Instance.SendOrDisconnect(realWriter);
            realWriter.Recycle();
        });

        writer.Recycle();
    }

    public static int GetTargetedClientId(string name)
    {
        int clientId = -1;
        var allClients = AmongUsClient.Instance.allObjectsFast;
        var allClientIds = allClients.Keys;

        foreach (uint id in allClientIds)
            if (clientId == -1 && allClients[id].name.Contains(name))
                clientId = (int)id;
        return clientId;
    }

    // This method is used to find the "GameManager" client which is now needed for synchronizing options
    public static int GetManagerClientId() => GetTargetedClientId("Manager");

    public static IGameOptions GetModifiedOptions(IEnumerable<GameOptionOverride> overrides)
    {
        IGameOptions clonedOptions = AUSettings.StaticOptions.DeepCopy();
        overrides.Where(o => o != null).ForEach(optionOverride => optionOverride.ApplyTo(clonedOptions));
        return clonedOptions;
    }

    public static void SendModifiedOptions(IEnumerable<GameOptionOverride> overrides, PlayerControl player)
    {
        SyncToPlayer(GetModifiedOptions(overrides), player);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Lotus.API.Player;
using Lotus.API.Reactive;
using Lotus.API.Reactive.HookEvents;
using Lotus.Extensions;
using Lotus.Managers.History;
using Lotus.Roles;
using Lotus.Roles.Exceptions;
using Lotus.Roles.Overrides;
using Lotus.RPC;
using Lotus.Server;
using Lotus.Statuses;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;

namespace Lotus.API.Odyssey;

public class MatchData
{
    public static MatchData Instance;
    internal ulong MatchID;

    public GameHistory GameHistory = new();
    public DateTime StartTime = DateTime.Now;


    public Dictionary<ulong, FrozenPlayer> FrozenPlayers = new();
    public VanillaRoleTracker VanillaRoleTracker = new();

    public HashSet<byte> UnreportableBodies = new();
    public int MeetingsCalled;
    public int EmergencyButtonsUsed;

    private static readonly Func<RemoteList<GameOptionOverride>> OptionOverrideListSupplier = GetGlobalOptions;

    public RoleData Roles = new();

    public FrozenPlayer? GetFrozenPlayer(PlayerControl? player)
    {
        return player == null || !FrozenPlayers.ContainsKey(player.GetGameID()) ? null : FrozenPlayers[player.GetGameID()];
    }

    public void Cleanup()
    {
        UnreportableBodies.Clear();
        Roles = new RoleData();
    }

    public class RoleData
    {
        public Dictionary<byte, CustomRole> PrimaryRoleDefinitions { get; } = new();
        public Dictionary<byte, List<CustomRole>> SecondaryRoleDefinitions { get; } = new();


        public Dictionary<byte, CustomRole> MainRoles = new();
        public Dictionary<byte, List<CustomRole>> SubRoles = new();


        private readonly Dictionary<byte, RemoteList<GameOptionOverride>> rolePersistentOverrides = new();

        public Remote<GameOptionOverride> AddOverride(byte playerId, GameOptionOverride @override)
        {
            return rolePersistentOverrides.GetOrCompute(playerId, OptionOverrideListSupplier).Add(@override);
        }

        public IEnumerable<GameOptionOverride> GetOverrides(byte playerId)
        {
            return rolePersistentOverrides.GetOrCompute(playerId, OptionOverrideListSupplier);
        }

        public IEnumerable<CustomRole> GetRoleDefinitions(byte playerId)
        {
            CustomRole primaryRoleDefinition = GetMainRole(playerId);
            return new List<CustomRole> { primaryRoleDefinition }.Concat(SecondaryRoleDefinitions.GetOrCompute(playerId, () => new List<CustomRole>()));
        }

        public CustomRole SetMainRole(byte playerId, CustomRole roleDefinition) => PrimaryRoleDefinitions[playerId] = roleDefinition;
        public void AddSubrole(byte playerId, CustomRole secondaryRoleDefinition) => SecondaryRoleDefinitions.GetOrCompute(playerId, () => new List<CustomRole>()).Add(secondaryRoleDefinition);

        public CustomRole GetMainRole(byte playerId) => PrimaryRoleDefinitions.GetOptional(playerId).OrElseGet(() => throw new NoSuchRoleException($"Player ID ({playerId}) does not have a primary role."));
        public List<CustomRole> GetSubroles(byte playerId) => SecondaryRoleDefinitions.GetOrCompute(playerId, () => new List<CustomRole>());
    }


    [ModRPC(ModCalls.SetPrimaryRole, RpcActors.Host, RpcActors.NonHosts, MethodInvocation.ExecuteBefore)]
    public static void AssignRole(PlayerControl player, CustomRole roleDefinition, bool sendToClient = false)
    {
        CustomRole assigned = Game.MatchData.Roles.SetMainRole(player.PlayerId, roleDefinition = roleDefinition.Instantiate(player));
        Game.MatchData.FrozenPlayers.GetOptional(player.GetGameID()).IfPresent(fp => fp.MainRole = assigned);
        if (Game.State is GameState.InLobby or GameState.InIntro) player.GetTeamInfo().MyRole = roleDefinition.RealRole;
        if (sendToClient) ProjectLotus.GameModeManager.CurrentGameMode.Assign(player, roleDefinition);
    }

    [ModRPC(ModCalls.SetSecondaryRole, RpcActors.Host, RpcActors.NonHosts, MethodInvocation.ExecuteBefore)]
    public static void AssignSubRole(PlayerControl player, CustomRole role, bool sendToClient = false)
    {
        CustomRole instantiated = role.Instantiate(player);
        Game.MatchData.Roles.AddSubrole(player.PlayerId, instantiated);
        if (sendToClient) ProjectLotus.GameModeManager.CurrentGameMode.Assign(player, role);
    }

    public static RemoteList<IStatus>? GetStatuses(PlayerControl player)
    {
        return Game.MatchData.FrozenPlayers.GetOptional(player.GetGameID()).Map(fp => fp.Statuses).OrElse(null!);
    }

    public static Remote<IStatus>? AddStatus(PlayerControl player, IStatus status, PlayerControl? infector = null)
    {
        return Game.MatchData.FrozenPlayers.GetOptional(player.GetGameID()).Map(fp => fp.Statuses).Transform(statuses =>
        {
            if (statuses.Any(s => s.Name == status.Name)) return null!;
            Remote<IStatus> remote = statuses.Add(status);
            Hooks.ModHooks.StatusReceivedHook.Propagate(new PlayerStatusReceivedHook(player, status, infector));
            return remote;
        }, () => null!);
    }

    // TODO make way better
    public static RemoteList<GameOptionOverride> GetGlobalOptions()
    {
        RemoteList<GameOptionOverride> globalOverrides = new() { new GameOptionOverride(Override.ShapeshiftCooldown, 0.1f) };
        if (AUSettings.ConfirmImpostor()) globalOverrides.Add(new GameOptionOverride(Override.ConfirmEjects, false));
        return globalOverrides;
    }
}
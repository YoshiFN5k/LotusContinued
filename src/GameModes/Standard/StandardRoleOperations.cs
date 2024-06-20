using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Lotus.API.Odyssey;
using Lotus.API.Player;
using Lotus.API.Reactive;
using Lotus.API.Reactive.HookEvents;
using Lotus.Extensions;
using Lotus.Factions;
using Lotus.Factions.Interfaces;
using Lotus.Options;
using Lotus.Roles;
using Lotus.Roles.Interfaces;
using Lotus.Roles.Internals;
using Lotus.Roles.Internals.Attributes;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Operations;
using Lotus.Roles.Overrides;
using VentLib.Networking.RPC;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace Lotus.GameModes.Standard;

public class StandardRoleOperations : RoleOperations
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(StandardRoleOperations));
    public GameMode ParentGameMode { get; }

    public StandardRoleOperations(GameMode parentGamemode)
    {
        ParentGameMode = parentGamemode;
    }

    public void Assign(CustomRole role, PlayerControl player)
    {
        ParentGameMode.Assign(player, role);
        // if (player.AmOwner) role.GUIProvider.Start();
    }

    public Relation Relationship(CustomRole source, CustomRole comparison) => source.Relationship(comparison);

    public Relation Relationship(CustomRole source, IFaction comparison) => comparison.Relationship(source.Faction);

    public void SyncOptions(PlayerControl target, IEnumerable<CustomRole> definitions, IEnumerable<GameOptionOverride>? overrides = null, bool deepSet = false)
    {
        if (target == null || !AmongUsClient.Instance.AmHost) return;

        overrides = CalculateOverrides(target, definitions, overrides);

        IGameOptions modifiedOptions = DesyncOptions.GetModifiedOptions(overrides);
        if (deepSet) RpcV3.Immediate(PlayerControl.LocalPlayer.NetId, RpcCalls.SyncSettings).Write(modifiedOptions).Send(target.GetClientId());
        DesyncOptions.SyncToPlayer(modifiedOptions, target);
    }

    public ActionHandle Trigger(LotusActionType action, PlayerControl? source, ActionHandle handle, params object[] parameters)
    {
        if (action is not LotusActionType.FixedUpdate)
        {
            Game.CurrentGameMode.Trigger(action, handle, parameters);
            if (handle.Cancellation is not (ActionHandle.CancelType.Soft or ActionHandle.CancelType.None)) return handle;
        }

        return TriggerFor(Players.GetPlayers().SelectMany(p => p.GetAllRoleDefinitions()), action, source, handle, parameters);
    }

    public ActionHandle TriggerFor(IEnumerable<CustomRole> recipients, LotusActionType action, PlayerControl? source, ActionHandle handle, params object[] parameters)
    {
        if (action is LotusActionType.FixedUpdate)
        {
            foreach (CustomRole role in recipients)
            {
                if (role.RoleActions.TryGetValue(action, out List<RoleAction>? actions) && !actions.IsEmpty())
                    actions[0].ExecuteFixed();
            }
            return handle;
        }

        parameters = parameters.AddToArray(handle);
        object[] globalActionParameters = parameters;
        if (!ReferenceEquals(source, null))
        {
            globalActionParameters = new object[parameters.Length + 1];
            Array.Copy(parameters, 0, globalActionParameters, 1, parameters.Length);
            globalActionParameters[0] = source;
        }

        IEnumerable<(RoleAction, AbstractBaseRole)> actionsAndDefinitions = recipients.SelectMany(cR => cR.GetActions(action)).OrderBy(t => t.Item1.Priority);
        foreach ((RoleAction roleAction, AbstractBaseRole roleDefinition) in actionsAndDefinitions)
        {
            PlayerControl myPlayer = roleDefinition.MyPlayer;
            if (handle.Cancellation is not (ActionHandle.CancelType.None or ActionHandle.CancelType.Soft)) continue;
            if (myPlayer == null) continue;
            if (!roleAction.CanExecute(myPlayer, source)) continue;

            try
            {
                if (roleAction.ActionType.IsPlayerAction())
                {
                    Hooks.PlayerHooks.PlayerActionHook.Propagate(new PlayerActionHookEvent(myPlayer, roleAction, parameters));
                    Trigger(LotusActionType.PlayerAction, myPlayer, handle, roleAction, parameters);
                }

                handle.ActionType = action;

                if (handle.Cancellation is not (ActionHandle.CancelType.None or ActionHandle.CancelType.Soft)) continue;

                roleAction.Execute(roleAction.Flags.HasFlag(ActionFlag.GlobalDetector) ? globalActionParameters : parameters);
            }
            catch (Exception e)
            {
                log.Exception($"Failed to execute RoleAction {action}.", e);
            }
        }

        return handle;
    }

    protected IEnumerable<GameOptionOverride> CalculateOverrides(PlayerControl player, IEnumerable<CustomRole> definitions, IEnumerable<GameOptionOverride>? overrides)
    {
        IEnumerable<GameOptionOverride> definitionOverrides = definitions.SelectMany(d => d.GetRoleOverrides());
        definitionOverrides = definitionOverrides.Concat(Game.MatchData.Roles.GetOverrides(player.PlayerId));
        if (overrides != null) definitionOverrides = definitionOverrides.Concat(overrides);
        return definitionOverrides;
    }

    public IRoleComponent Instantiate(GameMode setupHelper, PlayerControl player) => this;
}
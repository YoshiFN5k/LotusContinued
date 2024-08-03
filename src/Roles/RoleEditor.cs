using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using HarmonyLib;
using Lotus.API.Odyssey;
using Lotus.API.Reactive;
using Lotus.API.Reactive.HookEvents;
using Lotus.Factions;
using Lotus.Factions.Crew;
using Lotus.Factions.Impostors;
using Lotus.Factions.Interfaces;
using Lotus.Factions.Undead;
using Lotus.GUI;
using Lotus.Options;
using Lotus.Roles.Internals;
using Lotus.Roles.Internals.Attributes;
using Lotus.Roles.Overrides;
using Lotus.API.Stats;
using Lotus.Extensions;
using Lotus.Logging;
using Lotus.Roles.Builtins;
using Lotus.Roles.Builtins.Base;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Internals.Interfaces;
using Lotus.Utilities;
using UnityEngine;
using VentLib.Localization;
using VentLib.Options;
using VentLib.Options.UI;
using VentLib.Options.IO;
using VentLib.Utilities;
using VentLib.Utilities.Debug.Profiling;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;
using Lotus.Roles.RoleGroups.Vanilla;
using Lotus.API;
using Lotus.Addons;

using static Lotus.Roles.AbstractBaseRole;

namespace Lotus.Roles;

public abstract class RoleEditor
{
    internal AbstractBaseRole FrozenRole { get; }
    internal AbstractBaseRole ModdedRole = null!;
    internal CustomRole? RoleInstance;

    internal RoleEditor(AbstractBaseRole baseRole)
    {
        this.FrozenRole = baseRole;
    }

    internal AbstractBaseRole StartLink()
    {
        _editors.Clear();
        _editors.Add(this);
        this.ModdedRole = (AbstractBaseRole)Activator.CreateInstance(FrozenRole.GetType())!;
        this.ModdedRole.Editor = this;
        _editors.Clear();
        this.SetupActions();
        OnLink();
        return ModdedRole;
    }

    internal RoleEditor Instantiate(CustomRole role, PlayerControl player)
    {
        RoleEditor cloned = (RoleEditor)this.MemberwiseClone();
        cloned.RoleInstance = role;
        cloned.HookSetup(player);
        return cloned;
    }

    public virtual void HookSetup(PlayerControl myPlayer) { }

    public virtual RoleModifier HookModifier(RoleModifier modifier)
    {
        return modifier;
    }

    public virtual GameOptionBuilder HookOptions(GameOptionBuilder optionStream)
    {
        return optionStream;
    }

    public virtual void AddAction(RoleAction action)
    {
        FrozenRole.RoleActions[action.ActionType].Add(action);
    }

    public abstract void OnLink();

    private void PatchHook(object?[] args, ModifiedAction action, MethodInfo baseMethod)
    {
        if (action.Behaviour is ModifiedBehaviour.PatchBefore)
        {
            object? result = action.Method.InvokeAligned(args);
            if (action.Method.ReturnType == typeof(bool) && (result == null || (bool)result))
                baseMethod.InvokeAligned(args);
            return;
        }

        baseMethod.InvokeAligned(args);
        action.Method.InvokeAligned(args);
    }



    private void SetupActions()
    {
        this.GetType().GetMethods(BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .SelectMany(method => method.GetCustomAttributes<RoleActionAttribute>().Select(a => (a, method)))
            .Where(t => t.a.Subclassing || t.method.DeclaringType == this.GetType())
            .Select(t => t.Item1 is ModifiedActionAttribute modded ? new ModifiedAction(modded, t.method, this) : new RoleAction(t.Item1!, t.method, this))
            .Do(action =>
            {
                if (action is not ModifiedAction modded) ModdedRole.AddRoleAction(action);
                else
                {
                    List<RoleAction> currentActions = ModdedRole.RoleActions.GetValueOrDefault(action.ActionType, new List<RoleAction>());

                    switch (modded.Behaviour)
                    {
                        case ModifiedBehaviour.Replace:
                            currentActions.Clear();
                            currentActions.Add(modded);
                            break;
                        case ModifiedBehaviour.Addition:
                            currentActions.Add(modded);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    ModdedRole.RoleActions[action.ActionType] = currentActions;
                }
            });
    }
}
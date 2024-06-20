using AmongUs.GameOptions;
using Lotus.Options;
using Lotus.Roles.Internals.Attributes;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Overrides;
using UnityEngine;
using VentLib.Options.Game;

namespace Lotus.Roles.RoleGroups.Vanilla;

public class Phantom : Impostor
{
    protected float? VanishCooldown;
    protected float? VanishDuration;

    [RoleAction(LotusActionType.Attack, Subclassing = false)]
    public override bool TryKill(PlayerControl target) => base.TryKill(target);

    protected GameOptionBuilder AddShapeshiftOptions(GameOptionBuilder builder)
    {
        return builder.SubOption(sub => sub.Name("Vanish Cooldown")
                .AddFloatRange(0, 120, 2.5f, 12, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(f => VanishCooldown = f)
                .Build())
            .SubOption(sub => sub.Name("Vanish Duration")
                .Value(1f)
                .AddFloatRange(2.5f, 120, 2.5f, 6, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(f => VanishDuration = f)
                .Build());
    }

    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .VanillaRole(RoleTypes.Phantom)
            .RoleColor(Color.red)
            .CanVent(true)
            .OptionOverride(Override.PhantomVanishCooldown, () => VanishCooldown)
            .OptionOverride(Override.PhantomVanishDuration, () => VanishDuration);
}
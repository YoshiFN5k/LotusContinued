using AmongUs.GameOptions;
using TOHTOR.Roles.Internals;
using TOHTOR.Roles.Overrides;

namespace TOHTOR.Roles.RoleGroups.Vanilla;

public class Engineer: Crewmate
{
    protected float VentCooldown;
    protected float VentDuration;

    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .CanVent(true)
            .VanillaRole(RoleTypes.Engineer)
            .OptionOverride(Override.EngVentCooldown, VentCooldown)
            .OptionOverride(Override.EngVentDuration, VentDuration);
}
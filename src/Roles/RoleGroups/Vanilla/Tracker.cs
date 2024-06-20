using AmongUs.GameOptions;
using Lotus.Options;
using Lotus.Roles.Overrides;
using VentLib.Localization.Attributes;
using VentLib.Options.Game;

namespace Lotus.Roles.RoleGroups.Vanilla;

public class Tracker : Crewmate
{
    protected float TrackerCooldown;
    protected float TrackerDuration;
    protected float TrackerDelay;

    protected GameOptionBuilder AddTrackerOptions(GameOptionBuilder builder)
    {
        return builder.SubOption(sub => sub
                .Key("Tracker Cooldown")
                .Name(TrackerTranslations.Options.TrackerCooldown)
                .AddFloatRange(0, 120, 2.5f, 16, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(f => TrackerCooldown = f)
                .Build())
            .SubOption(sub => sub.Name(TrackerTranslations.Options.TrackerDuration)
                .Key("Tracker Duration")
                .Value(1f)
                .AddFloatRange(2.5f, 120, 2.5f, 6, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(f => TrackerDuration = f)
                .Build())
            .SubOption(sub => sub.Name(TrackerTranslations.Options.TrackerDelay)
                .Key("Tracker Update Delay")
                .Value(.5f)
                .AddFloatRange(0, 120, .5f, 6, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(f => TrackerDelay = f)
                .Build());
    }

    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .VanillaRole(RoleTypes.Tracker)
            .OptionOverride(Override.TrackerCooldown, () => TrackerCooldown)
            .OptionOverride(Override.TrackerDuration, () => TrackerDuration)
            .OptionOverride(Override.TrackerDelay, () => TrackerDelay);

    [Localized(nameof(Tracker))]
    public static class TrackerTranslations
    {
        public static class Options
        {
            [Localized(nameof(TrackerCooldown))]
            public static string TrackerCooldown = "Tracker Cooldown";

            [Localized(nameof(TrackerDuration))]
            public static string TrackerDuration = "Tracker Duration";

            [Localized(nameof(TrackerDelay))]
            public static string TrackerDelay = "Tracker Update Delay";
        }
    }
}
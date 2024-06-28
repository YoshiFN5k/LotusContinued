using AmongUs.GameOptions;
using Lotus.Options;
using Lotus.Roles.Overrides;
using VentLib.Localization.Attributes;
using VentLib.Options.UI;

namespace Lotus.Roles.RoleGroups.Vanilla;

public class Noisemaker : Crewmate
{
    protected bool? ImpostorsGetAlert;
    protected float? AlertDuration;

    protected GameOptionBuilder AddNoisemakerOptions(GameOptionBuilder builder)
    {
        return builder.SubOption(sub => sub
                .Key("Impostors Get Alert")
                .Name(NoisemakerTranslations.Options.ImpostorsGetAlert)
                .AddBoolean()
                .BindBool(b => ImpostorsGetAlert = b)
                .Build())
            .SubOption(sub => sub
                .Name(NoisemakerTranslations.Options.AlertDuration)
                .Key("Alert Duration")
                .Value(.5f)
                .AddFloatRange(0, 120, .5f, 6, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(f => AlertDuration = f)
                .Build());
    }

    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .VanillaRole(RoleTypes.Noisemaker)
            .OptionOverride(Override.NoiseAlertDuration, () => AlertDuration)
            .OptionOverride(Override.NoiseImpGetAlert, () => ImpostorsGetAlert);

    [Localized(nameof(Noisemaker))]
    public static class NoisemakerTranslations
    {
        public static class Options
        {
            [Localized(nameof(ImpostorsGetAlert))]
            public static string ImpostorsGetAlert = "Impostors Get Alert";

            [Localized(nameof(AlertDuration))]
            public static string AlertDuration = "Alerts Duration";
        }
    }
}
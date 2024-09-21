using VentLib.Localization.Attributes;

namespace Lotus.Roles;

[Localized("Options.Roles")]
public class RoleTranslations
{
    [Localized("Maximum")]
    public static string MaximumText = "Maximum";

    [Localized("SubsequentChance")]
    public static string SubsequentChanceText = "Subsequent Chance";

    [Localized("CanVent")]
    public static string CanVent = "Can Vent";

    [Localized(nameof(CanSabotage))]
    public static string CanSabotage = "Can Sabotage";
}
using TOHTOR.Options.Roles;
using VentLib.Localization.Attributes;

namespace TOHTOR.Options;

[Localized("Options")]
public class RoleOptions
{
    public static MadmateOptions MadmateOptions = null!;
    public static NeutralOptions NeutralOptions = null!;
    public static SubroleOptions SubroleOptions = null!;

    internal static MadmateOptions LoadMadmateOptions() => MadmateOptions ??= new MadmateOptions();

    internal static NeutralOptions LoadNeutralOptions() => NeutralOptions ??= new NeutralOptions();

    internal static SubroleOptions LoadSubroleOptions() => SubroleOptions ??= new SubroleOptions();
}
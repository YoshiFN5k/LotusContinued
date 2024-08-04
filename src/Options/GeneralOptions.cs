using System.Collections.Generic;
using Lotus.Options.General;
using VentLib.Localization.Attributes;
using VentLib.Options.UI;
using VentLib.Utilities.Attributes;
using VentLib.Options;

namespace Lotus.Options;

[Localized(ModConstants.Options)]
[LoadStatic]
public static class GeneralOptions
{
    public static OptionManager MainOptionManager = OptionManager.GetManager(file: "options.txt");
    public static AdminOptions AdminOptions;
    public static DebugOptions DebugOptions;
    public static GameplayOptions GameplayOptions;
    public static MayhemOptions MayhemOptions;
    public static MeetingOptions MeetingOptions;
    public static MiscellaneousOptions MiscellaneousOptions;
    public static SabotageOptions SabotageOptions;

    public static List<GameOption> AllOptions = new();

    static GeneralOptions()
    {
        AdminOptions = new AdminOptions();
        // AllOptions.AddRange(AdminOptions.AllOptions);

        GameplayOptions = new GameplayOptions();
        AllOptions.AddRange(GameplayOptions.AllOptions);

        SabotageOptions = new SabotageOptions();
        AllOptions.AddRange(SabotageOptions.AllOptions);

        MeetingOptions = new MeetingOptions();
        AllOptions.AddRange(MeetingOptions.AllOptions);

        MayhemOptions = new MayhemOptions();
        AllOptions.AddRange(MayhemOptions.AllOptions);

        MiscellaneousOptions = new MiscellaneousOptions();
        // AllOptions.AddRange(MiscellaneousOptions.AllOptions);

        DebugOptions = new DebugOptions();
        // AllOptions.AddRange(DebugOptions.AllOptions);

        AllOptions.AddRange(RoleOptions.LoadMadmateOptions().AllOptions);
        AllOptions.AddRange(RoleOptions.LoadNeutralOptions().AllOptions);
        AllOptions.AddRange(RoleOptions.LoadSubroleOptions().AllOptions);

        AllOptions.ForEach(o => o.Register(MainOptionManager));
    }
}
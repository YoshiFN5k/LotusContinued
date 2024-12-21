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
    public static OptionManager StandardOptionManager = OptionManager.GetManager(file: "options.txt");
    public static OptionManager CaptureOptionManager = OptionManager.GetManager(file: "ctf.txt");
    public static OptionManager ColorwarsOptionManager = OptionManager.GetManager(file: "colorwars.txt");

    public static AdminOptions AdminOptions;
    public static DebugOptions DebugOptions;
    public static GameplayOptions GameplayOptions;
    public static MayhemOptions MayhemOptions;
    public static MeetingOptions MeetingOptions;
    public static MiscellaneousOptions MiscellaneousOptions;
    public static SabotageOptions SabotageOptions;

    public static List<GameOption> StandardOptions = new();

    static GeneralOptions()
    {
        AdminOptions = new AdminOptions();
        // StandardOptions.AddRange(AdminOptions.AllOptions);

        GameplayOptions = new GameplayOptions();
        StandardOptions.AddRange(GameplayOptions.AllOptions);

        SabotageOptions = new SabotageOptions();
        StandardOptions.AddRange(SabotageOptions.AllOptions);

        MeetingOptions = new MeetingOptions();
        StandardOptions.AddRange(MeetingOptions.AllOptions);

        MayhemOptions = new MayhemOptions();
        StandardOptions.AddRange(MayhemOptions.AllOptions);

        MiscellaneousOptions = new MiscellaneousOptions();
        // StandardOptions.AddRange(MiscellaneousOptions.AllOptions);

        DebugOptions = new DebugOptions();
        // StandardOptions.AddRange(DebugOptions.AllOptions);

        StandardOptions.AddRange(RoleOptions.LoadMadmateOptions().AllOptions);
        StandardOptions.AddRange(RoleOptions.LoadNeutralOptions().AllOptions);
        StandardOptions.AddRange(RoleOptions.LoadSubroleOptions().AllOptions);

        StandardOptions.ForEach(o => StandardOptionManager.Register(o, OptionLoadMode.LoadOrCreate));
    }
}
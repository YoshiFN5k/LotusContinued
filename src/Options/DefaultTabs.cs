using System.Collections.Generic;
using Lotus.Utilities;
using VentLib.Options.UI;
using VentLib.Options.UI.Controllers;
using VentLib.Options.UI.Tabs;
using VentLib.Utilities.Attributes;

namespace Lotus.Options;

[LoadStatic]
public class DefaultTabs
{
    public static MainSettingTab GeneralTab = new("Lotus Settings", "Modify all the Project Lotus settings here!");

    public static GameOptionTab ImpostorsTab = new("Impostor Settings", () => AssetLoader.LoadSprite("Lotus.assets.TabIcons.ImpostorsIconNew.png", 270, true));

    public static GameOptionTab CrewmateTab = new("Crewmate Settings", () => AssetLoader.LoadSprite("Lotus.assets.TabIcons.CrewmatesIconNew.png", 270, true));
    // TODO: add title colors for these other tabs
    public static GameOptionTab NeutralTab = new("Neutral Settings", () => AssetLoader.LoadSprite("Lotus.assets.TabIcons.NeutralsIconNew.png", 270, true));

    //public static GameOptionTab SubrolesTab = new("Subrole Settings", "Lotus.assets.TabIcon_Addons.png");

    public static GameOptionTab MiscTab = new("Misc Settings", () => AssetLoader.LoadSprite("Lotus.assets.TabIcons.MiscIconNew.png", 270, true));

    public static GameOptionTab HiddenTab = new("Hidden", () => AssetLoader.LoadSprite("Lotus.assets.GeneralIcon.png", 300, true));

    public static List<GameOptionTab> StandardTabs = new() { ImpostorsTab, CrewmateTab, NeutralTab, MiscTab };

    static DefaultTabs()
    {
        // GameOptionController.AddTab(GeneralTab);
        // GameOptionController.AddTab(ImpostorsTab);
        // GameOptionController.AddTab(CrewmateTab);
        // GameOptionController.AddTab(NeutralTab);
        // GameOptionController.AddTab(MiscTab);
        SettingsOptionController.SetMainTab(GeneralTab);
    }
}
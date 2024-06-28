using System.Collections.Generic;
using Lotus.API.Odyssey;
using Lotus.Options;
using Lotus.Roles;
using Lotus.Victory;
using VentLib.Options.UI.Tabs;
using VentLib.Utilities.Extensions;

namespace Lotus.GameModes.Standard;

public class StandardGameMode : GameMode
{
    public static StandardGameMode Instance;

    public override string Name { get; set; } = "Standard";
    public new StandardRoleOperations RoleOperations { get; }
    public new StandardRoleManager RoleManager { get; }
    public new MatchData MatchData { get; set; }

    public StandardGameMode() : base()
    {
        Instance = this;
        MatchData = new();

        RoleOperations = new(this);
        RoleManager = new();
    }

    // public override void Activate() => RoleManager.RoleHolder.AllRoles.ForEach(RoleManager.RegisterRole);

    // public override void Deactivate() => EnabledTabs().ForEach(tb => tb.GetOptions().ForEach(tb.RemoveOption));

    public override void Assign(PlayerControl player, CustomRole role)
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerable<GameOptionTab> EnabledTabs() => DefaultTabs.All;
    public override MainSettingTab MainTab() => DefaultTabs.GeneralTab;

    public override void Setup()
    {
        throw new System.NotImplementedException();
    }

    public override void SetupWinConditions(WinDelegate winDelegate)
    {
        throw new System.NotImplementedException();
    }

    public override void AssignRoles(List<PlayerControl> players)
    {
        throw new System.NotImplementedException();
    }
}
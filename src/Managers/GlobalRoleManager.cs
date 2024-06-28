using System.Collections.Generic;
using Lotus.GameModes;
using Lotus.GameModes.Standard;
using Lotus.Logging;
using Lotus.Roles;
using VentLib.Options;
using VentLib.Utilities.Attributes;

namespace Lotus.Managers;
public class GlobalRoleManager : StandardRoleManager
{
    public static OptionManager RoleOptionManager = OptionManager.GetManager(file: "role_options.txt");

    public static GlobalRoleManager Instance;
    public new RoleHolder RoleHolder { get; }

    public GlobalRoleManager()
    {
        DevLogger.Log("running GlobalRoleManager");
        Instance = this;
    }

    public override IEnumerable<CustomRole> AllCustomRoles() => OrderedCustomRoles.GetValues();
    internal override bool IsGlobal => true;
}
using System.Collections.Generic;
using Lotus.GameModes.Standard;
using Lotus.Roles;
using VentLib.Options;
using VentLib.Utilities.Attributes;

namespace Lotus.Managers;
public class GlobalRoleManager : StandardRoleManager
{
    public OptionManager RoleOptionManager = OptionManager.GetManager(file: "role_options.txt");

    public static GlobalRoleManager Instance { get; set; }

    public override IEnumerable<CustomRole> AllCustomRoles() => OrderedCustomRoles.GetValues();
    internal override bool IsGlobal => true;
}
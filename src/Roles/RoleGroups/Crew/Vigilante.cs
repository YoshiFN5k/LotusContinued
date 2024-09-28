using Lotus.Factions;
using Lotus.Roles.Builtins;
using UnityEngine;

namespace Lotus.Roles.RoleGroups.Crew;


public class Vigilante : GuesserRole
{
    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .Faction(FactionInstances.Crewmates)
            .RoleColor(new Color(0.89f, 0.88f, 0.52f));
}
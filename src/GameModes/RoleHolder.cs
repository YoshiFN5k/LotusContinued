using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lotus.Roles;

namespace Lotus.GameModes;
public abstract class RoleHolder : IRoleHolder
{
    public abstract List<Action> FinishedCallbacks();

    public List<CustomRole> MainRoles { get; set; }

    public List<CustomRole> ModifierRoles { get; set; }

    public List<CustomRole> SpecialRoles { get; set; }

    public List<CustomRole> AllRoles { get; set; }

    public RoleHolder()
    {

    }

    public void AddOnFinishCall(Action action) => this.FinishedCallbacks().Add(action);
    public void AddRole(CustomRole role) => this.AllRoles.Add(role);
    public bool Intialized
    {
        get { return _initialized; }
        set
        {
            if (_initialized != true)
                FinishedCallbacks().All(fc => { fc(); return true; });

            _initialized = true;
        }
    }
    public bool _initialized;
}
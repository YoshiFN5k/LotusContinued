using Lotus.Roles.Internals.Attributes;
using Lotus.Roles.Internals.Enums;
using VentLib.Options.UI;

namespace Lotus.Roles.RoleGroups.Vanilla;

public class Morphling : Shapeshifter
{
    [RoleAction(LotusActionType.Attack)]
    public override bool TryKill(PlayerControl target) => base.TryKill(target);

    protected override GameOptionBuilder RegisterOptions(GameOptionBuilder optionStream) =>
        AddShapeshiftOptions(base.RegisterOptions(optionStream));
}
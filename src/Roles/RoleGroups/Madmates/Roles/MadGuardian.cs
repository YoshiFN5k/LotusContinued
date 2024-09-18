using Lotus.Roles.Interactions.Interfaces;
using Lotus.Roles.Internals;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Internals.Attributes;

namespace Lotus.Roles.RoleGroups.Madmates.Roles;

public class MadGuardian : MadCrewmate
{
    [RoleAction(LotusActionType.Interaction)]
    private void MadGuardianAttacked(PlayerControl actor, Interaction interaction, ActionHandle handle)
    {
        if (interaction.Intent is not (IFatalIntent or IHostileIntent)) return;
        handle.Cancel();
    }

    [Localized(nameof(MadGuardian))]
    public static class Translations
    {
        [Localized(ModConstants.Options)]
        public static class Options
        {
            [Localized(nameof(ImpostorVision))]
            public static string ImpostorVision = "Has Impostor Vision";

            [Localized(nameof(CanVent))]
            public static string CanVent = "Can Vent";

            [Localized(nameof(OverrideTasks))]
            public static string OverrideTasks = "Override Mad Guardian's Tasks";

            [Localized(nameof(AllowCommonTasks))]
            public static string AllowCommonTasks = "Allow Common Tasks";

            [Localized(nameof(LongTasks))]
            public static string LongTasks = "Mad Guardian Long Tasks";

            [Localized(nameof(ShortTasks))]
            public static string ShortTasks = "Mad Guardian Short Tasks";

        }
    }
}
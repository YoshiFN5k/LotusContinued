using Lotus.GUI;
using Lotus.GUI.Name;
using Lotus.GUI.Name.Holders;
using Lotus.Roles.Internals.Attributes;
using Lotus.API;
using Lotus.API.Odyssey;
using Lotus.Extensions;
using Lotus.Options;
using UnityEngine;
using VentLib.Localization.Attributes;
using Lotus.Roles.Internals.Enums;
using VentLib.Options.UI;
using VentLib.Utilities;

namespace Lotus.Roles.RoleGroups.NeutralKilling;

[Localized("Roles.Werewolf")]
public class Werewolf : NeutralKillingBase
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(Werewolf));
    private bool rampaging;
    private bool canVentNormally;
    private bool canVentDuringRampage;

    [UIComponent(UI.Cooldown)]
    private Cooldown rampageDuration;

    [UIComponent(UI.Cooldown)]
    private Cooldown rampageCooldown;

    [Localized("Rampage")]
    private string rampagingString = "RAMPAGING";

    protected override void PostSetup()
    {
        base.PostSetup();
        MyPlayer.NameModel().GetComponentHolder<CooldownHolder>()[1].SetPrefix(RoleColor.Colorize(rampagingString + " "));
    }

    [RoleAction(LotusActionType.Attack)]
    public new bool TryKill(PlayerControl target) => rampaging && base.TryKill(target);

    [RoleAction(LotusActionType.OnPet)]
    private void EnterRampage()
    {
        if (rampageDuration.NotReady() || rampageCooldown.NotReady()) return;
        log.Trace($"{MyPlayer.GetNameWithRole()} Starting Rampage");
        rampaging = true;
        rampageDuration.Start();
        Async.Schedule(ExitRampage, rampageDuration.Duration);
    }

    [RoleAction(LotusActionType.RoundEnd)]
    private void ExitRampage()
    {
        log.Trace($"{MyPlayer.GetNameWithRole()} Ending Rampage");
        rampaging = false;
        rampageCooldown.Start();
    }

    public override bool CanVent() => canVentNormally || rampaging;

    protected override GameOptionBuilder RegisterOptions(GameOptionBuilder optionStream) =>
        base.RegisterOptions(optionStream)
            .SubOption(sub => sub
                .KeyName("Rampage Kill Cooldown", Translations.Options.RampageKillCooldown)
                .AddFloatRange(1f, 60f, 2.5f, 2, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(f => KillCooldown = f)
                .Build())
            .SubOption(sub => sub
                .KeyName("Rampage Cooldown", Translations.Options.RampageCooldown)
                .AddFloatRange(5f, 120f, 2.5f, 14, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(rampageCooldown.SetDuration)
                .Build())
            .SubOption(sub => sub
                .KeyName("Rampage Duration", Translations.Options.RampageDuration)
                .AddFloatRange(5f, 120f, 2.5f, 4, GeneralOptionTranslations.SecondsSuffix)
                .BindFloat(rampageDuration.SetDuration)
                .Build())
            .SubOption(sub => sub
                .KeyName("Can Vent Normally", Translations.Options.CanVentNormally)
                .AddOnOffValues(false)
                .BindBool(b => canVentNormally = b)
                .ShowSubOptionPredicate(o => !(bool)o)
                .SubOption(sub2 => sub2
                    .KeyName("Can Vent in Rampage", Translations.Options.CanVentInRampage)
                    .BindBool(b => canVentDuringRampage = b)
                    .AddOnOffValues()
                    .Build())
                .Build());

    protected override RoleModifier Modify(RoleModifier roleModifier)
    {
        RoleAbilityFlag flags = RoleAbilityFlag.CannotSabotage | RoleAbilityFlag.UsesPet;
        if (!(canVentNormally || canVentDuringRampage)) flags |= RoleAbilityFlag.CannotVent;

        return base.Modify(roleModifier)
            .RoleAbilityFlags(flags)
            .RoleColor(new Color(0.66f, 0.4f, 0.16f));
    }

    [Localized(nameof(Werewolf))]
    public static class Translations
    {
        [Localized(ModConstants.Options)]
        public static class Options
        {
            [Localized(nameof(RampageKillCooldown))]
            public static string RampageKillCooldown = "Rampage Kill Cooldown";

            [Localized(nameof(RampageCooldown))]
            public static string RampageCooldown = "Rampage Cooldown";

            [Localized(nameof(RampageDuration))]
            public static string RampageDuration = "Rampage Duration";

            [Localized(nameof(CanVentNormally))]
            public static string CanVentNormally = "Can Vent Normally";

            [Localized(nameof(CanVentInRampage))]
            public static string CanVentInRampage = "Can Vent in Rampage";
        }
    }
}
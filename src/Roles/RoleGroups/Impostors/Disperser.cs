using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Lotus.API.Odyssey;
using Lotus.GUI;
using Lotus.GUI.Name;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Internals.Attributes;
using Lotus.Roles.RoleGroups.Vanilla;
using Lotus.Utilities;
using Lotus.API;
using Lotus.Extensions;
using Lotus.Options;
using UnityEngine;
using VentLib.Localization.Attributes;
using VentLib.Options.UI;
using VentLib.Utilities.Extensions;
using Lotus.API.Player;

namespace Lotus.Roles.RoleGroups.Impostors;

public class Disperser : Impostor
{
    private bool disperserDispersed;

    [UIComponent(UI.Cooldown)]
    private Cooldown abilityCooldown;

    [RoleAction(LotusActionType.Attack)]
    public new bool TryKill(PlayerControl target) => base.TryKill(target);

    [RoleAction(LotusActionType.OnPet)]
    private void DispersePlayers()
    {
        if (abilityCooldown.NotReady()) return;
        abilityCooldown.Start();
        List<Vent> vents = Object.FindObjectsOfType<Vent>().ToList();
        if (vents.Count == 0) return;
        Players.GetAlivePlayers()
            .Where(p => disperserDispersed || p.PlayerId != MyPlayer.PlayerId)
            .Do(p =>
            {
                Vector2 ventPosition = vents.GetRandom().transform.position;
                Utils.Teleport(p.NetTransform, new Vector2(ventPosition.x, ventPosition.y + 0.3636f));
            });
    }

    protected override GameOptionBuilder RegisterOptions(GameOptionBuilder optionStream) =>
        base.RegisterOptions(optionStream)
            .SubOption(sub => sub
                .KeyName("Disperse Cooldown", Translations.Options.DisperseCooldown)
                .BindFloat(abilityCooldown.SetDuration)
                .AddFloatRange(0, 120, 2.5f, 5, GeneralOptionTranslations.SecondsSuffix)
                .Build())
            .SubOption(sub => sub.KeyName("Disperser Gets Dispersed", TranslationUtil.Colorize(Translations.Options.DisperserGetsDispersed, RoleColor))
                .AddOnOffValues()
                .BindBool(b => disperserDispersed = b)
                .Build());

    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .RoleAbilityFlags(RoleAbilityFlag.UsesPet);


    [Localized(nameof(Disperser))]
    private static class Translations
    {
        [Localized(ModConstants.Options)]
        public static class Options
        {
            [Localized(nameof(DisperseCooldown))]
            public static string DisperseCooldown = "Disperse Cooldown";

            [Localized(nameof(DisperserGetsDispersed))]
            public static string DisperserGetsDispersed = "Disperser::0 Gets Dispersed";
        }
    }
}
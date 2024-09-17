using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Lotus.API.Odyssey;
using Lotus.Factions;
using Lotus.GUI;
using Lotus.GUI.Name;
using Lotus.GUI.Name.Components;
using Lotus.GUI.Name.Holders;
using Lotus.Managers.History.Events;
using Lotus.Roles.Events;
using Lotus.Roles.Interactions;
using Lotus.Roles.Internals;
using Lotus.Roles.Internals.Enums;
using Lotus.Roles.Internals.Attributes;
using Lotus.Roles.Overrides;
using Lotus.Roles.RoleGroups.Vanilla;
using Lotus.Extensions;
using Lotus.Options;
using UnityEngine;
using VentLib.Logging;
using VentLib.Networking.RPC;
using VentLib.Options.UI;
using VentLib.Utilities;
using VentLib.Utilities.Optionals;
using Lotus.API.Player;
using VentLib.Localization.Attributes;

namespace Lotus.Roles.RoleGroups.Impostors;

public class Swooper : Impostor
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(Swooper));
    private bool canVentNormally;
    private bool canBeSeenByAllied;
    private bool remainInvisibleOnKill;
    private Optional<Vent> initialVent = null!;

    [UIComponent(UI.Cooldown)]
    private Cooldown swoopingDuration = null!;

    [UIComponent(UI.Cooldown)]
    private Cooldown swooperCooldown = null!;

    [RoleAction(LotusActionType.Attack)]
    public new bool TryKill(PlayerControl target)
    {
        if (!remainInvisibleOnKill || swoopingDuration.IsReady()) return base.TryKill(target);
        MyPlayer.RpcMark(target);
        InteractionResult result = MyPlayer.InteractWith(target, new LotusInteraction(new FatalIntent(true, () => new DeathEvent(target, MyPlayer)), this));
        Game.MatchData.GameHistory.AddEvent(new KillEvent(MyPlayer, target, result is InteractionResult.Proceed));
        return result is InteractionResult.Proceed;
    }

    protected override void PostSetup()
    {
        MyPlayer.NameModel().GetComponentHolder<CooldownHolder>()[0].SetTextColor(new Color(0.2f, 0.63f, 0.29f));
        LiveString swoopingString = new(() => swoopingDuration.NotReady() ? "Swooping" : "", Color.red);
        MyPlayer.NameModel().GetComponentHolder<TextHolder>().Add(new TextComponent(swoopingString, new[] { GameState.Roaming }, viewers: GetUnaffected));
        MyPlayer.NameModel().GetComponentHolder<TextHolder>().Add(new TextComponent(LiveString.Empty, GameState.Roaming, ViewMode.Replace, MyPlayer));
    }

    [RoleAction(LotusActionType.VentEntered)]
    private void SwooperInvisible(Vent vent, ActionHandle handle)
    {
        if (swooperCooldown.NotReady() || swoopingDuration.NotReady())
        {
            if (canVentNormally) return;
            if (swoopingDuration.IsReady()) handle.Cancel();
            return;
        }

        List<PlayerControl> unaffected = GetUnaffected();
        initialVent = Optional<Vent>.Of(vent);

        swoopingDuration.Start();
        Game.MatchData.GameHistory.AddEvent(new GenericAbilityEvent(MyPlayer, $"{MyPlayer.name} began swooping."));
        Async.Schedule(() => KickFromVent(vent, unaffected), 0.4f);
        Async.Schedule(EndSwooping, swoopingDuration.Duration);
    }

    private void KickFromVent(Vent vent, List<PlayerControl> unaffected)
    {
        if (MyPlayer.AmOwner) MyPlayer.MyPhysics.BootFromVent(vent.Id);
        RpcV3.Immediate(MyPlayer.MyPhysics.NetId, RpcCalls.BootFromVent).WritePacked(vent.Id).SendInclusive(unaffected.Select(p => p.GetClientId()).ToArray());
    }

    private void EndSwooping()
    {
        int ventId = initialVent.Map(v => v.Id).OrElse(0);
        log.Trace($"Ending Swooping (ID: {ventId})");

        Async.Schedule(() => MyPlayer.MyPhysics.RpcBootFromVent(ventId), 0.4f);
        swooperCooldown.Start();
    }

    private List<PlayerControl> GetUnaffected() => Players.GetAllPlayers().Where(p => !p.IsAlive() || canBeSeenByAllied && p.Relationship(MyPlayer) is Relation.FullAllies).AddItem(MyPlayer).ToList();

    protected override GameOptionBuilder RegisterOptions(GameOptionBuilder optionStream) => base.RegisterOptions(optionStream)
        .SubOption(sub => sub
            .KeyName("Invisibility Cooldown", Translations.Options.InvisibilityCooldown)
            .AddFloatRange(5, 120, 2.5f, 16, GeneralOptionTranslations.SecondsSuffix)
            .BindFloat(swooperCooldown.SetDuration)
            .Build())
        .SubOption(sub => sub
            .KeyName("Swooping Duration", Translations.Options.SwoopingDuration)
            .AddFloatRange(5, 60, 1f, 5, GeneralOptionTranslations.SecondsSuffix)
            .BindFloat(swoopingDuration.SetDuration)
            .Build())
        .SubOption(sub => sub
            .KeyName("Can be Seen By Allies", Translations.Options.SeenByAllies)
            .AddOnOffValues()
            .BindBool(b => canBeSeenByAllied = b)
            .Build())
        .SubOption(sub => sub
            .KeyName("Can Vent During Cooldown", Translations.Options.VentDuringCooldown)
            .AddOnOffValues(false)
            .BindBool(b => canVentNormally = b)
            .Build())
        .SubOption(sub => sub
            .KeyName("Remain Invisible on Kill", Translations.Options.InvisibleOnKill)
            .AddOnOffValues()
            .BindBool(b => remainInvisibleOnKill = b)
            .Build());

    protected override RoleModifier Modify(RoleModifier roleModifier) =>
        base.Modify(roleModifier)
            .OptionOverride(new IndirectKillCooldown(KillCooldown, () => remainInvisibleOnKill && swoopingDuration.NotReady()));

    [Localized(nameof(Swooper))]
    public static class Translations
    {
        [Localized(ModConstants.Options)]
        public static class Options
        {
            [Localized(nameof(InvisibilityCooldown))]
            public static string InvisibilityCooldown = "Invisibility Cooldown";

            [Localized(nameof(SwoopingDuration))]
            public static string SwoopingDuration = "Swooping Duration";

            [Localized(nameof(SeenByAllies))]
            public static string SeenByAllies = "Can Be Seen By Allies";

            [Localized(nameof(VentDuringCooldown))]
            public static string VentDuringCooldown = "Can Vent During Cooldown";

            [Localized(nameof(InvisibleOnKill))]
            public static string InvisibleOnKill = "Remain Invisible on Kill";
        }
    }
}
using System.Collections.Generic;
using Lotus;
using Lotus.Extensions;
using Lotus.GUI;
using Lotus.Options;
using Lotus.Patches.Network;
using Lotus.Roles.Builtins;
using UnityEngine;
using VentLib.Localization.Attributes;
using VentLib.Options.IO;
using VentLib.Options.UI;

namespace Lotus.Options.General;

[Localized(ModConstants.Options)]
public class AdminOptions
{
    private static Color _optionColor = GameMaster.GMColor;
    private static List<GameOption> additionalOptions = new();

    // ReSharper disable once InconsistentNaming
    public bool HostGM;
    public bool AutoKick;
    public bool KickPlayersWithoutFriendcodes;
    public int KickPlayersUnderLevel;
    public bool KickMobilePlayers;

    public int AutoStartPlayerThreshold;
    public int AutoStartMaxTime = -1;
    public int AutoStartGameCountdown;
    public bool AutoPlayAgain;

    public Cooldown AutoCooldown = new();
    public bool AutoStartEnabled;
    public List<GameOption> AllOptions = new();

    public AdminOptions()
    {
        AllOptions.Add(new GameOptionTitleBuilder()
            .Title(AdminOptionTranslations.AdminTitle)
            .Color(_optionColor)
            .IsHeader(false)
            .Build());

        AllOptions.Add(new GameOptionBuilder()
            .AddBoolean(false)
            .Name(AdminOptionTranslations.HostGmText)
            .Key("Host GM").Color(_optionColor)
            .BindBool(b => HostGM = b)
            .IsHeader(true)
            .BuildAndRegister());

        // TODO: repeat offenders
        AllOptions.Add(new GameOptionBuilder()
            .AddBoolean()
            .Builder("Chat AutoKick", _optionColor)
            .Name(AdminOptionTranslations.AutoKickText)
            .BindBool(b => AutoKick = b)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddBoolean(false)
            .Builder("Kick Players Without Friendcode", _optionColor)
            .Name(AdminOptionTranslations.AutoKickNoFriendCodeText)
            .BindBool(b => KickPlayersWithoutFriendcodes = b)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddIntRange(1, 100, 1)
            .Value(v => v.Text(GeneralOptionTranslations.DisabledText).Value(0).Color(Color.red).Build())
            .Builder("Kick Players Under Level", _optionColor)
            .Name(AdminOptionTranslations.AutoKickUnderLevel)
            .BindInt(i => KickPlayersUnderLevel = i)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddBoolean(false)
            .Builder("Kick Mobile Players", _optionColor)
            .Name(AdminOptionTranslations.AutoKickMobile)
            .BindBool(b => KickMobilePlayers = b)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddBoolean(false)
            .Builder("Auto Start", _optionColor)
            .Name(AdminOptionTranslations.AutoStartText)
            .BindBool(b =>
            {
                AutoStartEnabled = b;
                if (GameStartManager.Instance != null && !b) GameStartManager.Instance.ResetStartState();
            })
            .ShowSubOptionPredicate(b => (bool)b)
            .SubOption(sub2 => sub2
                .AddIntRange(5, 15, suffix: " " + AdminOptionTranslations.AutoStartSuffix)
                .Value(v => v.Text(GeneralOptionTranslations.OffText).Value(-1).Color(Color.red).Build())
                .Name(AdminOptionTranslations.AutoStartPlayerThreshold)
                .IOSettings(io => io.UnknownValueAction = ADEAnswer.Allow)
                .BindInt(i => AutoStartPlayerThreshold = i)
                .Build())
            .SubOption(sub2 => sub2
                .AddIntRange(30, 540, 15, 0, GeneralOptionTranslations.SecondsSuffix)
                .Value(v => v.Text(GeneralOptionTranslations.OffText).Value(-1).Color(Color.red).Build())
                .Name(AdminOptionTranslations.AutoStartMaxWaitTime)
                .IOSettings(io => io.UnknownValueAction = ADEAnswer.Allow)
                .BindInt(i =>
                {
                    if (LobbyBehaviour.Instance == null) return;
                    AutoStartMaxTime = i;
                    if (i == -1) AutoCooldown.Finish();
                    else
                    {
                        AutoCooldown.SetDuration(i);
                        AutoCooldown.Start();
                    }
                    PlayerJoinPatch.CheckAutostart();
                })
                .Build())
            .SubOption(sub2 => sub2
                .AddIntRange(0, 20, 2, 5, GeneralOptionTranslations.SecondsSuffix)
                .Name(AdminOptionTranslations.AutoStartGameCountdown)
                .BindInt(i => AutoStartGameCountdown = i)
                .Build())
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddBoolean()
            .Builder("Auto Play Again", _optionColor)
            .Name(AdminOptionTranslations.AutoPlayAgain)
            .BindBool(b => AutoPlayAgain = b)
            .BuildAndRegister());

        additionalOptions.ForEach(o =>
        {
            o.Register();
            AllOptions.Add(o);
        });
    }

    /// <summary>
    /// Adds additional options to be registered when this group of options is loaded. This is mostly used for ordering
    /// in the main menu, as options passed in here will be rendered along with this group.
    /// </summary>
    /// <param name="option">Option to render</param>
    public static void AddAdditionalOption(GameOption option)
    {
        additionalOptions.Add(option);
    }

    [Localized("Admin")]
    private class AdminOptionTranslations
    {
        [Localized("SectionTitle")]
        public static string AdminTitle = "Host Options";

        [Localized("HostGM")]
        public static string HostGmText = "Host GM";

        [Localized("AutoKick")]
        public static string AutoKickText = "Chat Auto Kick";

        [Localized("AutoKickNoFriendcode")]
        public static string AutoKickNoFriendCodeText = "Kick Players w/o Friendcodes";

        [Localized("AutoKickLevel")]
        public static string AutoKickUnderLevel = "Kick Players Under Level";

        [Localized(nameof(AutoKickMobile))]
        public static string AutoKickMobile = "Kick Mobile Players";

        // Auto Start
        [Localized("AutoStart")]
        public static string AutoStartText = "Auto Start";

        [Localized(nameof(AutoStartPlayerThreshold))]
        public static string AutoStartPlayerThreshold = "Player Threshold";

        [Localized(nameof(AutoStartMaxWaitTime))]
        public static string AutoStartMaxWaitTime = "Maximum Wait Time";

        [Localized(nameof(AutoStartGameCountdown))]
        public static string AutoStartGameCountdown = "Game Countdown";

        [Localized("AutoStartOptionSuffix")]
        public static string AutoStartSuffix = "Players";

        [Localized(nameof(AutoPlayAgain))]
        public static string AutoPlayAgain = "Auto Play Again";
    }

}

public enum BannedMobileDevice
{

}
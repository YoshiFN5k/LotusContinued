using System.Collections.Generic;
using Lotus.Extensions;
using Lotus.GUI;
using Lotus.Roles;
using UnityEngine;
using VentLib.Localization.Attributes;
using VentLib.Options.UI;
using VentLib.Options.IO;

namespace Lotus.Options.General;

[Localized(ModConstants.Options)]
public class MiscellaneousOptions
{
    private static Color _optionColor = new(1f, 0.75f, 0.81f);
    private static List<GameOption> additionalOptions = new();

    public string AssignedPet = null!;
    public int ChangeNameUsers;
    public int ChangeColorAndLevelUsers;
    public bool AutoDisplayLastResults;
    public bool AutoDisplayCOD;
    public int SuffixMode;
    public bool ColoredNameMode;

    public List<GameOption> AllOptions = new();

    public MiscellaneousOptions()
    {

        AllOptions.Add(new GameOptionTitleBuilder()
            .Title(MiscOptionTranslations.MiscOptionTitle)
            .Color(_optionColor)
            .Build());

        GameOptionBuilder AddPets(GameOptionBuilder b)
        {
            foreach ((string? key, string? value) in ModConstants.Pets) b = b.Value(v => v.Text(key).Value(value).Build());
            return b;
        }

        AllOptions.Add(AddPets(new GameOptionBuilder())
            .Builder("Assigned Pet", _optionColor)
            .Name(MiscOptionTranslations.AssignedPetText)
            .IsHeader(true)
            .BindString(s => AssignedPet = s)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .Value(v => v.Value(0).Text(GeneralOptionTranslations.OffText).Color(Color.red).Build())
            .Value(v => v.Value(1).Text(GeneralOptionTranslations.FriendsText).Color(new Color(0.85f, 0.66f, 1f)).Build())
            .Value(v => v.Value(2).Text(GeneralOptionTranslations.EveryoneText).Color(Color.green).Build())
            .Builder("Allow /name", _optionColor)
            .Name(MiscOptionTranslations.AllowNameCommand)
            .BindInt(b => ChangeNameUsers = b)
            .IOSettings(io => io.UnknownValueAction = ADEAnswer.UseDefault)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .Value(v => v.Value(0).Text(GeneralOptionTranslations.OffText).Color(Color.red).Build())
            .Value(v => v.Value(1).Text(GeneralOptionTranslations.FriendsText).Color(new Color(0.85f, 0.66f, 1f)).Build())
            .Value(v => v.Value(2).Text(GeneralOptionTranslations.EveryoneText).Color(Color.green).Build())
            .Builder("Allow /color and /level", _optionColor)
            .Name(MiscOptionTranslations.AllowColorAndLevelCommand)
            .BindInt(b => ChangeColorAndLevelUsers = b)
            .IOSettings(io => io.UnknownValueAction = ADEAnswer.UseDefault)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddOnOffValues()
            .Builder("Auto Display Results", _optionColor)
            .Name(MiscOptionTranslations.AutoDisplayResultsText)
            .BindBool(b => AutoDisplayLastResults = b)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddOnOffValues()
            .Builder("Auto Display Cause of Death", _optionColor)
            .Name(MiscOptionTranslations.AutoDisplayCauseOfDeath)
            .BindBool(b => AutoDisplayCOD = b)
            .BuildAndRegister());

        AllOptions.Add(new GameOptionBuilder()
            .AddBoolean(false)
            .Builder("Color Names", _optionColor)
            .Name(MiscOptionTranslations.ColorNames)
            .BindBool(b => ColoredNameMode = b)
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

    private GameOptionBuilder Builder(string key) => new GameOptionBuilder().Key(key).Color(_optionColor);

    [Localized("Miscellaneous")]
    private static class MiscOptionTranslations
    {
        [Localized("SectionTitle")]
        public static string MiscOptionTitle = "Miscellaneous Options";

        [Localized("AssignedPet")]
        public static string AssignedPetText = "Assigned Pet";

        [Localized(nameof(AllowNameCommand))]
        public static string AllowNameCommand = "Allow /name";

        [Localized(nameof(AllowColorAndLevelCommand))]
        public static string AllowColorAndLevelCommand = "Allow /color and /level";

        [Localized("AutoDisplayResults")]
        public static string AutoDisplayResultsText = "Auto Display Results";

        [Localized(nameof(AutoDisplayCauseOfDeath))]
        public static string AutoDisplayCauseOfDeath = "Auto Display Cause of Death";

        [Localized("SuffixMode")]
        public static string SuffixModeText = "Suffix Mode";

        [Localized(nameof(ColorNames))]
        public static string ColorNames = "Color Names";
    }

}
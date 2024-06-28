using System;
using System.Collections.Generic;
using System.Linq;
using Lotus.API.Odyssey;
using Lotus.API.Reactive;
using Lotus.GameModes.Standard;
using Lotus.Victory;
using VentLib.Options;
using VentLib.Options.UI;
using VentLib.Options.Events;
using VentLib.Options.UI.Tabs;
using VentLib.Utilities.Extensions;
using Lotus.Options;

namespace Lotus.GameModes;

// As we move to the future we're going to try to use instances for managers rather than making everything static
public class GameModeManager
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(GameModeManager));

    private const string GameModeManagerStartHook = nameof(GameModeManager);

    internal readonly List<IGameMode> GameModes = new();

    public IGameMode CurrentGameMode
    {
        get => currentGameMode!;
        set
        {
            currentGameMode?.InternalDeactivate();
            currentGameMode = value;
            currentGameMode?.InternalActivate();
        }
    }

    private IGameMode? currentGameMode;
    private GameOption gamemodeOption = null!;

    public GameModeManager()
    {
        Hooks.GameStateHooks.GameStartHook.Bind(GameModeManagerStartHook, _ => CurrentGameMode.SetupWinConditions(Game.GetWinDelegate()));
    }

    public void SetGameMode(int id)
    {
        if (currentGameMode?.GetType() == GameModes[id].GetType()) return;
        CurrentGameMode = GameModes[id];
        log.High($"Setting GameMode {CurrentGameMode.Name}", "GameMode");
    }

    public IEnumerable<IGameMode> GetGameModes() => GameModes;
    public IGameMode GetGameMode(int id) => GameModes[id];
    public IGameMode? GetGameMode(Type type) => GameModes.FirstOrDefault(t => t.GetType() == type);

    public void Setup()
    {
        GameOptionBuilder builder = new GameOptionBuilder();

        GameModes.AddRange(new List<IGameMode>()
        {
            new StandardGameMode()
        });

        // currentGameMode = GameModes[0];

        for (int i = 0; i < GameModes.Count; i++)
        {
            IGameMode gameMode = GameModes[i];
            var index = i;
            builder.Value(v => v.Text(gameMode.Name).Value(index).Build());
        }

        gamemodeOption = builder.Name("GameMode").IsHeader(true).BindInt(SetGameMode).BuildAndRegister();
        DefaultTabs.GeneralTab.AddOption(new GameOptionTitleBuilder()
            .Title("Gamemode Selection")
            .Build());
        DefaultTabs.GeneralTab.AddOption(gamemodeOption);
        GeneralOptions.AllOptions.ForEach(DefaultTabs.GeneralTab.AddOption);
        // GameOptionController.RegisterEventHandler(ce =>
        // {
        //     if (ce is not OptionOpenEvent) return;
        //     GameOptionController.ClearTabs();
        //     currentGameMode?.EnabledTabs().ForEach(GameOptionController.AddTab);
        // });
    }

    public void StartGame(WinDelegate winDelegate)
    {
        CurrentGameMode.CoroutineManager.Start();
        CurrentGameMode.SetupWinConditions(winDelegate);
    }
}
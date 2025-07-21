using BaCon;
using R3;

public static class MainMenuRegistrations
{
    public static void Register(DIContainer container, MainMenuEnterParams maneMenuEnterParams)
    {
        var gameStateProvider = container.Resolve<IGameStateProvider>();
        var gameState = gameStateProvider.GameState;
        var settingsProvider = container.Resolve<ISettingsProvider>();
        var gameSettings = settingsProvider.GameSettings;

        container.RegisterInstance(AppConstants.EXIT_SCENE_REQUEST_TAG, new Subject<Unit>());

        // CmdHandler Registration
        var cmd = new CommandProcessor(gameStateProvider);

        container.RegisterInstance<ICommandProcessor>(cmd);

        //Service registration
    }
}
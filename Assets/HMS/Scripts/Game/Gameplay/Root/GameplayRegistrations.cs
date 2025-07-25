using BaCon;
using R3;

public static class GameplayRegistrations
{
    public static void Register(DIContainer container, GameplayEnterParams gameplayEnterParams)
    {
        var gameStateProvider = container.Resolve<IGameStateProvider>();
        var gameState = gameStateProvider.GameState;
        var settingsProvider = container.Resolve<ISettingsProvider>();
        var gameSettings = settingsProvider.GameSettings;

        container.RegisterInstance(AppConstants.EXIT_SCENE_REQUEST_TAG, new Subject<Unit>());

        // CmdHandler Registration
        var cmd = new CommandProcessor(gameStateProvider);

        // World
        // Natural
        cmd.RegisterHandler(new CmdCreatePerlinMapHandler(gameState));
        cmd.RegisterHandler(new CmdCreateTemperatureMapHandler(gameState));
        cmd.RegisterHandler(new CmdCreateRiverHandler(gameState));
        cmd.RegisterHandler(new CmdCreateBiomesHandler(gameState));
        // Infrastructure
        cmd.RegisterHandler(new CmdCreateQualityOfCityLocationsMapHandler(gameState));
        cmd.RegisterHandler(new CmdCreateCityHandler(gameState));
        // Political


        // Resource 
        cmd.RegisterHandler(new CmdResourcesAddHandler(gameState));
        cmd.RegisterHandler(new CmdResourcesSpendHandler(gameState));

        container.RegisterInstance<ICommandProcessor>(cmd);

        // Service registration
        container.RegisterFactory(_ => new WorldService(gameplayEnterParams.WorldId, gameState.Worlds, cmd)).AsSingle();
        container.RegisterFactory(_ => new ResourcesService(gameState.Resources, cmd)).AsSingle();
    }
}
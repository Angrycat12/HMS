using BaCon;

public static class GameplayViewModelsRegistrations
{
    public static void Register(DIContainer container)
    {
        container.RegisterFactory(c => new GameplayUIManager(container)).AsSingle();
        container.RegisterFactory(c => new UIGameplayRootViewModel()).AsSingle();
        container.RegisterFactory(c => new WorldGameplayRootViewModel(container.Resolve<WorldService>())).AsSingle();
    }
}
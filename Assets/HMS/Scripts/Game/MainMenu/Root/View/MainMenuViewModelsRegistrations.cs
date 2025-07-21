using BaCon;

public static class MainMenuViewModelsRegistrations
{
    public static void Register(DIContainer container)
    {
        container.RegisterFactory(c => new MainMenuUIManager(container)).AsSingle();
        container.RegisterFactory(c => new UIMainMenuRootViewModel()).AsSingle();
    }
}
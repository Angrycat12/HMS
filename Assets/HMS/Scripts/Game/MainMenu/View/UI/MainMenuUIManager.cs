using BaCon;
using R3;

public class MainMenuUIManager : UIManager
{
    private readonly Subject<Unit> _exitSceneRequest;

    public MainMenuUIManager(DIContainer container) : base(container)
    {
        _exitSceneRequest = container.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
    }
    
    public ScreenMainMenuViewModel OpenScreenMainMenu()
    {
        var viewModel = new ScreenMainMenuViewModel(this, _exitSceneRequest);
        var rootUI = Container.Resolve<UIMainMenuRootViewModel>();

        rootUI.OpenScreen(viewModel);

        return viewModel;
    }
}

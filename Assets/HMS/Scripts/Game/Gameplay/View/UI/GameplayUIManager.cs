using BaCon;
using R3;

public class GameplayUIManager : UIManager
{
    private readonly Subject<Unit> _exitSceneRequest;

    public GameplayUIManager(DIContainer container) : base(container)
    {
        _exitSceneRequest = container.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
    }
    
    public ScreenGameplayViewModel OpenScreenGameplay()
    {
        var viewModel = new ScreenGameplayViewModel(this, _exitSceneRequest);
        var rootUI = Container.Resolve<UIGameplayRootViewModel>();

        rootUI.OpenScreen(viewModel);

        return viewModel;
    }
}

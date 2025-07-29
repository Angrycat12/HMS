using BaCon;
using R3;
using UnityEngine;

public class GameplayEntryPoint : MonoBehaviour
{
    [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;
    [SerializeField] private WorldGameplayRootBinder _worldRootBinder;
    private Coroutines _coroutines; 

    public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEnterParams enterParams, Coroutines coroutine)
    {
        _coroutines = coroutine;
        
        GameplayRegistrations.Register(gameplayContainer, enterParams);
        var gameplayViewModelsContainer = new DIContainer(gameplayContainer);
        GameplayViewModelsRegistrations.Register(gameplayViewModelsContainer);

        InitWorld(gameplayViewModelsContainer);
        InitUI(gameplayViewModelsContainer);

        var mainMenuEnterParams = new MainMenuEnterParams();
        var exitParams = new GameplayExitParams(mainMenuEnterParams);
        var exitSceneRequest = gameplayContainer.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
        var exitToMainMenuSceneSignal = exitSceneRequest.Select(_ => exitParams);

        return exitToMainMenuSceneSignal;
    }

    private void InitWorld(DIContainer viewsContainer)
    {
        _coroutines.StartCoroutine(_worldRootBinder.Bind(viewsContainer.Resolve<WorldGameplayRootViewModel>()));
    }

    private void InitUI(DIContainer viewsContainer)
    {
        // Создали UI для сцены (это было)
        var uiRoot = viewsContainer.Resolve<UIRootView>();

        uiRoot.HideLoadingScreen();
        
        var uiSceneRootBinder = Instantiate(_sceneUIRootPrefab);
        uiRoot.AttachSceneUI(uiSceneRootBinder.gameObject);
        
        // Запрашиваем рутовую вью модель и пихаем ее в баиндер, который создали
        var uiSceneRootViewModel = viewsContainer.Resolve<UIGameplayRootViewModel>();
        uiSceneRootBinder.Bind(uiSceneRootViewModel);
        
        // можно открывать окошки
        var uiManager = viewsContainer.Resolve<GameplayUIManager>();
        uiManager.OpenScreenGameplay();
    }

}

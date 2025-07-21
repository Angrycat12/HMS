using BaCon;
using R3;
using UnityEngine;

public class MainMenuEntryPoint : MonoBehaviour
{
    [SerializeField] private UIMainMenuRootBinder _sceneUIRootPrefab;

    public Observable<MainMenuExitParams> Run(DIContainer mainMenuContainer, MainMenuEnterParams enterParams)
    {
        MainMenuRegistrations.Register(mainMenuContainer, enterParams);
        var mainMenuViewModelsContainer = new DIContainer(mainMenuContainer);
        MainMenuViewModelsRegistrations.Register(mainMenuViewModelsContainer);

        InitUI(mainMenuViewModelsContainer);

        var gameplayEnterParams = new GameplayEnterParams();
        var mainMenuExitParams = new MainMenuExitParams(gameplayEnterParams);
        var exitSceneRequest = mainMenuContainer.Resolve<Subject<Unit>>(AppConstants.EXIT_SCENE_REQUEST_TAG);
        var exitToGameplaySceneSignal = exitSceneRequest.Select(_ => mainMenuExitParams);

        return exitToGameplaySceneSignal;
    }

    private void InitUI(DIContainer viewsContainer)
    {
        // Создали UI для сцены (это было)
        var uiRoot = viewsContainer.Resolve<UIRootView>();
        var uiSceneRootBinder = Instantiate(_sceneUIRootPrefab);
        uiRoot.AttachSceneUI(uiSceneRootBinder.gameObject);
        
        // Запрашиваем рутовую вью модель и пихаем ее в баиндер, который создали
        var uiSceneRootViewModel = viewsContainer.Resolve<UIMainMenuRootViewModel>();
        uiSceneRootBinder.Bind(uiSceneRootViewModel);
        
        // можно открывать окошки
        var uiManager = viewsContainer.Resolve<MainMenuUIManager>();
        uiManager.OpenScreenMainMenu();
    }
}

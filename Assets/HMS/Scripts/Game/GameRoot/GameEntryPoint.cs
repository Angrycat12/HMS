using System.Collections;
using BaCon;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEntryPoint
{
    private static GameEntryPoint _instance;
    private Coroutines _coroutines;
    private UIRootView _uiRoot;
    private readonly DIContainer _rootContainer = new();
    private DIContainer _cachedSceneContainer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void AutostartGame()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        _instance = new GameEntryPoint();
        _instance.RunGame();
    }

    private GameEntryPoint()
    {
        _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
        Object.DontDestroyOnLoad(_coroutines.gameObject);

        var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
        _uiRoot = Object.Instantiate(prefabUIRoot);
        Object.DontDestroyOnLoad(_uiRoot.gameObject);
        _rootContainer.RegisterInstance(_uiRoot);

        // Settings
        var settingsProvider = new SettingsProvider();
        _rootContainer.RegisterInstance<ISettingsProvider>(settingsProvider); 

        var gameStateProvider = new PlayerPrefsGameStateProvider();
        gameStateProvider.LoadSettingsState();
        _rootContainer.RegisterInstance<IGameStateProvider>(gameStateProvider);

        _rootContainer.RegisterFactory(_ => new SteamService()).AsSingle();
    }

    private void RunGame()
    {
        #if UNITY_EDITOR

        var sceneName = SceneManager.GetActiveScene().name;

        if(sceneName == Scenes.MAINMENU)
        {
            _coroutines.StartCoroutine(LoadAndStartMainMenu(new MainMenuEnterParams()));
            return;
        }
        if(sceneName == Scenes.GAMEPLAY)
        {
            _coroutines.StartCoroutine(LoadAndStartGameplay(new GameplayEnterParams()));
            return;
        }
        if(sceneName != Scenes.BOOT)
        {
            return;
        }

        #endif

        _coroutines.StartCoroutine(LoadAndStartMainMenu(new MainMenuEnterParams()));
    }

    private IEnumerator LoadAndStartMainMenu(MainMenuEnterParams enterParams)
    {
        _uiRoot.ShowLoadingScreen();
        _cachedSceneContainer?.Dispose();

        yield return LoadScene(Scenes.BOOT);
        yield return LoadScene(Scenes.MAINMENU);

        var sceneEntryPoint = Object.FindFirstObjectByType<MainMenuEntryPoint>();
        var mainMenuContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
        sceneEntryPoint.Run(mainMenuContainer, enterParams).Subscribe(mainMenuExitParams => 
        {
            var targetSceneName = mainMenuExitParams.TargetSceneEnterParams.SceneName;
            if (targetSceneName == Scenes.GAMEPLAY)
            {
                _coroutines.StartCoroutine(LoadAndStartGameplay(mainMenuExitParams.TargetSceneEnterParams.As<GameplayEnterParams>()));
            }
        });

        _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadAndStartGameplay(GameplayEnterParams enterParams)
    {
        _uiRoot.ShowLoadingScreen();
        _cachedSceneContainer?.Dispose();

        yield return LoadScene(Scenes.BOOT);
        yield return LoadScene(Scenes.GAMEPLAY);

        var isGameStateLoaded = false;
        _rootContainer.Resolve<IGameStateProvider>().LoadGameState().Subscribe(_ => isGameStateLoaded = true);
        yield return new WaitUntil(() => isGameStateLoaded);

        var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
        var gameplayContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
        sceneEntryPoint.Run(gameplayContainer, enterParams).Subscribe(gameplayExitParams => 
        {
            _coroutines.StartCoroutine(LoadAndStartMainMenu(new MainMenuEnterParams()));
        });

        // _uiRoot.HideLoadingScreen();
    }

    private IEnumerator LoadScene(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}
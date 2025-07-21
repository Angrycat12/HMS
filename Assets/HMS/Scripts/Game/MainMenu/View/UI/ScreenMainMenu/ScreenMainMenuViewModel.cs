using R3;

public class ScreenMainMenuViewModel : WindowViewModel
{
    private readonly MainMenuUIManager _uiManager;
        private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenMainMenu";

        public ScreenMainMenuViewModel(MainMenuUIManager uiManager, Subject<Unit> exitSceneRequest)
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
        }

}
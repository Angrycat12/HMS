using R3;

public class ScreenGameplayViewModel : WindowViewModel
{
    private readonly GameplayUIManager _uiManager;
        private readonly Subject<Unit> _exitSceneRequest;
        public override string Id => "ScreenGameplay";

        public ScreenGameplayViewModel(GameplayUIManager uiManager, Subject<Unit> exitSceneRequest)
        {
            _uiManager = uiManager;
            _exitSceneRequest = exitSceneRequest;
        }

}
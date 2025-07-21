using UnityEditor;

public class GameplayEnterParams : SceneEnterParams
{
    public readonly int WorldId = 0;

    public GameplayEnterParams() : base(Scenes.GAMEPLAY)
    {
        //WorldName = worldName;
    }
}


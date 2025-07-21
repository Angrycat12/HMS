using System.Linq;
using ObservableCollections;
using R3;

public class GameStateProxy
{
    private readonly GameState _gameState;
    public ObservableList<World> Worlds { get; } = new();
    public ObservableList<Resource> Resources { get; } = new();

    public GameStateProxy(GameState gameState)
    {
        _gameState = gameState;

        InitWorlds(gameState);
        InitResources(gameState);
    }

    private void InitWorlds(GameState gameState)
    {
        gameState.Worlds.ForEach(worldData => Worlds.Add(new World(worldData)));

        Worlds.ObserveAdd().Subscribe(e =>
        {
            var addedWorld = e.Value;
            gameState.Worlds.Add(addedWorld.Origin);
        });
        
        Worlds.ObserveRemove().Subscribe(e =>
        {
            var removedWorld = e.Value;
            gameState.Worlds.Remove(removedWorld.Origin);
        });
    }

    private void InitResources(GameState gameState)
    {
        gameState.Resources.ForEach(resourceData => Resources.Add(new Resource(resourceData)));
        
        Resources.ObserveAdd().Subscribe(e =>
        {
            var addedResource = e.Value;
            gameState.Resources.Add(addedResource.Origin);
        });
        
        Resources.ObserveRemove().Subscribe(e =>
        {
            var removedResource = e.Value;
            var removedResourceData = gameState.Resources.FirstOrDefault(b => b.ResourceType == removedResource.ResourceType);
            gameState.Resources.Remove(removedResourceData);
        });
    }
}
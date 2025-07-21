using System.Linq;
using UnityEngine;

public class CmdCreateVegetationMapHandler: ICommandHandler<CmdCreateVegetationMap>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateVegetationMapHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public bool Handle(CmdCreateVegetationMap command)
    {
        float[,] answer = new float[command.Width, command.Height];
        
        for (int x = 0; x < command.Width; x++)
        {
            for (int y = 0; y < command.Height; y++)
            {
                float xCoord = ((float)x + command.Seed) / command.Width * command.Scale;
                float yCoord = ((float)y + command.Seed) / command.Height * command.Scale;
                
                answer[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId).VegetationMap.OnNext(answer);
        return true;
    }
}
using System;
using System.Linq;
using UnityEngine;

public class CmdCreateTemperatureMapHandler: ICommandHandler<CmdCreateTemperatureMap>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateTemperatureMapHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public bool Handle(CmdCreateTemperatureMap command)
    {
        float[,] answer = new float[command.Width, command.Height];

        // Find argument a = y0 / ((x2 - x1) / 2) ^ 2
        double a = 1 / Math.Pow(command.Height / 2, 2);
        
        for (int x = 0; x < command.Width; x++)
        {
            for (int y = 0; y < command.Height; y++)
            {
                // find parabola y = a * (x - x1) * (x - x2)
                answer[x, y] = (float)(-a * y * (y - command.Height));
            }
        } 

        _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId).TemperatureMap.OnNext(answer);
        
        return true;
    }
}
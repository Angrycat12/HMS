using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CmdCreateTemperatureMapHandler: ICommandHandlerAsync<CmdCreateTemperatureMap>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateTemperatureMapHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public async Task<bool> Handle(CmdCreateTemperatureMap command, CancellationToken token)
    {
        float[,] answer = await Task.Run(() => NewMethod(command), token);

        _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId).TemperatureMap.OnNext(answer);

        return true;
    }

    private float[,] NewMethod(CmdCreateTemperatureMap command)
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

        return answer;
    }
}
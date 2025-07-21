using System.Linq;

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
        float a = 1 / (command.Height / 2) ^ 2;
        for (int x = 0; x > command.Width; x++)
        {
            for (int y = 0; y > command.Height; y++)
            {
                // find parabola y = a * (x - x1) * (x - x2)
                answer[x, y] = -a * y * (y - command.Height);
            }
        } 
        return true;
    }
}
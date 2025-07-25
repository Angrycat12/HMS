using UnityEngine;

public class CmdCreatePerlinMapHandler: ICommandHandler<CmdCreatePerlinMap>
{
    private readonly GameStateProxy _gameState;

    public CmdCreatePerlinMapHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public bool Handle(CmdCreatePerlinMap command)
    {
        float[,] answer = new float[command.Width, command.Height];

        for (int x = 0; x < command.Width; x++)
        {
            for (int y = 0; y < command.Height; y++)
            {
                float value = 0f;
                float currentAmplitude = command.Amplitude;
                float currentFrequency = command.Frequency;

                for (int octave = 0; octave < command.Octaves; octave++)
                {
                    
                    float xCoord = (x + command.Period) / command.Width * currentFrequency;
                    float yCoord = (y + command.Period) / command.Height * currentFrequency;

                    
                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord);

                    // Добавляем значение шума в итоговое, с учетом амплитуды
                    value += perlinValue * currentAmplitude;

                    // Уменьшаем амплитуду и увеличиваем частоту для следующей октавы
                    currentAmplitude /= 2f;  // Каждая последующая октава в 2 раза слабее
                    currentFrequency *= 2f;  // Каждая следующая октава имеет в 2 раза большую частоту
                }

                // Применение амплитуды и периодичности
                answer[x, y] = value;
            }
        }

        command.Map.OnNext(answer);

        return true;
    }
}
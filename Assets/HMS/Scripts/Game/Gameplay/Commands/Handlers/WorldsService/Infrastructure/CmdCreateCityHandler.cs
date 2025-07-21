class CmdCreateCityHandler : ICommandHandler<CmdCreateCity>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateCityHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public bool Handle(CmdCreateCity command)
    {
        float[,] quality_of_city_locations = command.QualityOfCityLocations;
        if (command.QualityOfCityLocations is null)
        {
            quality_of_city_locations = CreateQualityOfCityLocations(command);
        }

        for (int x = 0; x < command.Width; x++)
        {
            for (int y = 0; y < command.Height; y++)
            {
                if (quality_of_city_locations[x, y] == 0)
                {
                    continue;
                }
            }
        }
        return true;
    }

    private float[,] CreateQualityOfCityLocations(CmdCreateCity command)
    {
        float[,] quality_of_city_locations = new float[command.Width, command.Height];

        int countParametrs = 4;

        for (int x = 0; x < command.Width; x++)
        {
            for (int y = 0; y < command.Height; y++)
            {
                float quality_of_city_location = 0f;
                if (command.HeightMap[x, y] <= command.WaterLevel)
                {
                    quality_of_city_locations[x, y] = quality_of_city_location;
                    continue;
                }

                quality_of_city_location += (float)((-1.25 * command.HeightMap[x, y]) + 1.25 / countParametrs); // inverse linear dependence [water level, 1] -> [1, 0]
                quality_of_city_location += (command.HumidityMap[x, y]) / countParametrs; // i don't known
                quality_of_city_location += (command.TemperatureMap[x, y]) / countParametrs; // linear dependence [0, 1] -> [0, 1]
                quality_of_city_location += (-1 * command.VegetationMap[x, y] + 1) / countParametrs; // inverse linear dependence [0, 1] -> [1, 0]

                quality_of_city_locations[x, y] = quality_of_city_location;
            }
        }
        return quality_of_city_locations;
    }
}
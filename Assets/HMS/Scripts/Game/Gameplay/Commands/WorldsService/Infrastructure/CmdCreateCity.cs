class CmdCreateCity : ICommand
{
    public readonly int WorldId;
    public readonly int Width;
    public readonly int Height;
    public readonly int Count;
    public readonly float WaterLevel;
    public readonly float[,] HeightMap;
    public readonly float[,] HumidityMap;
    public readonly float[,] TemperatureMap;
    public readonly float[,] VegetationMap;
    public readonly float[,] QualityOfCityLocations;
    public readonly int[] RiverMap;

    public CmdCreateCity(int worldId, int width, int height, int count, float waterLevel, float[,] heightMap, float[,] humidityMap, float[,] temperatureMap, float[,] vegetationMap, float[,] qualityOfCityLocations, int[] riverMap)
    {
        WorldId = worldId;
        Width = width;
        Height = height;
        Count = count;
        WaterLevel = waterLevel;
        HeightMap = heightMap;
        HumidityMap = humidityMap;
        TemperatureMap = temperatureMap;
        VegetationMap = vegetationMap;
        QualityOfCityLocations = qualityOfCityLocations;
        RiverMap = riverMap;
    }
}
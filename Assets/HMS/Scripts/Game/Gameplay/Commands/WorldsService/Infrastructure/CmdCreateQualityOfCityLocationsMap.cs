public class CmdCreateQualityOfCityLocationsMap : ICommand
{
    public readonly int WorldId;
    public readonly int Width;
    public readonly int Height;
    public readonly float WaterLevel;
    public readonly float[,] HeightMap;
    public readonly float[,] HumidityMap;
    public readonly float[,] TemperatureMap;
    public readonly float[,] VegetationMap;
    public readonly int[] RiverMap;

    public CmdCreateQualityOfCityLocationsMap(int worldId, int width, int height, float waterLevel, float[,] heightMap, float[,] humidityMap, float[,] temperatureMap, float[,] vegetationMap, int[] riverMap)
    {
        WorldId = worldId;
        Width = width;
        Height = height;
        WaterLevel = waterLevel;
        HeightMap = heightMap;
        HumidityMap = humidityMap;
        TemperatureMap = temperatureMap;
        VegetationMap = vegetationMap;
        RiverMap = riverMap;
    }
}
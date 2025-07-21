public class CmdCreateBiomes : ICommand
{
    public readonly int WorldId;
    public readonly int Width;
    public readonly int Height;
    public readonly int CountPoints;
    public readonly int LloydRelaxations;
    public readonly float[,] HeightMap;
    public readonly float[,] HumidityMap;
    public readonly float[,] TemperatureMap;
    public readonly float[,] VegetationMap;
    public CmdCreateBiomes(int worldId, int width, int height, int countPoints, int lloydRelaxations, float[,] heightMap, float[,] humidityMap, float[,] temperatureMap, float[,] vegetationMap)
    {
        WorldId = worldId;
        Width = width;
        Height = height;
        CountPoints = countPoints;
        LloydRelaxations = lloydRelaxations;
        HeightMap = heightMap;
        HumidityMap = humidityMap;
        TemperatureMap = temperatureMap;
        VegetationMap = vegetationMap;
    }
}
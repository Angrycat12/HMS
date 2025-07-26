using System.Collections.Generic;

public class CmdCreateRoad : ICommand
{
    public readonly int WorldId;
    public readonly int Width;
    public readonly int Height;
    public readonly float WaterLevel;
    public readonly float[,] HeightMap;
    public readonly List<City> Cities;

    public CmdCreateRoad(int worldId, int width, int height, float waterLevel,float[,] heightMap, List<City> cities)
    {
        WorldId = worldId;
        Width = width;
        Height = height;
        WaterLevel = waterLevel;
        HeightMap = heightMap;
        Cities = cities;
    }
}
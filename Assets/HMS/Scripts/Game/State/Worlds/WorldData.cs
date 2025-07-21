using System;
using System.Collections.Generic;

public class WorldData
{
    public int id;
    public string name;
    public int seed;
    public int height;
    public int width;
    public float waterLevel;
    public float[,] heightMap;
    public float[ , ] temperatureMap;
    public float[ , ] humidityMap;
    public float[ , ] vegetationMap;
    public List<RiverData> rivers;
    public List<BiomeData> biomes;
    public List<Region> regions;
    public List<Country> countries;
}
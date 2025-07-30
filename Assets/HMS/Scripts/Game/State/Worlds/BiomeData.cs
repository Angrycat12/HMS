using System.Collections.Generic;
using Noise;


public class BiomeData
{
    public int Id;
    public VoronoiCell Polygon;
    public List<int[]> Points; // list cord in map

    public BiomeData(int id, VoronoiCell polygon, List<int[]> points)
    {
        Id = id;
        Polygon = polygon;
        Points = points;
    }
}
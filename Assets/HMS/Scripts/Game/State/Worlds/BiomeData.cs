using System.Collections.Generic;
using Angrycat.Noise;


public class BiomeData
{
    public int Id;
    public BiomeType Type;
    public VoronoiCell Polygon;
    public List<int[]> Points; // list cord in map

    public BiomeData(int id, VoronoiCell polygon, List<int[]> points)
    {
        Id = id;
        Polygon = polygon;
        Points = points;
    }
}
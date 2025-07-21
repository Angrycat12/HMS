using System.Collections.Generic;
using DelaunayVoronoi;

public class BiomeData
{
    public int Id;
    public Polygon Polygon;
    public List<int[]> Points; // list cord in map

    public BiomeData(int id, Polygon polygon, List<int[]> points)
    {
        Id = id;
        Polygon = polygon;
        Points = points;
    }
}
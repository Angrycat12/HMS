using System.Collections.Generic;
using UnityEngine;

class CmdCreateRiver : ICommand
{
    public readonly int WorldId;
    public readonly List<Vector2Int> SourceRiver;
    public readonly float[,] HeightMap;
    public readonly float WaterLevel;

    public CmdCreateRiver(int worldId,List<Vector2Int> sourceRiver, float[,] heightMap, float waterLevel)
    {
        WorldId = worldId;
        SourceRiver = sourceRiver;
        HeightMap = heightMap;
        WaterLevel = waterLevel;
    }
}
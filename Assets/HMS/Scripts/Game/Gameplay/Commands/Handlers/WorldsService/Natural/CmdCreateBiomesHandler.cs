using System.Collections.Generic;
using System.Linq;
using DelaunayVoronoi;
using UnityEngine;

public class CmdCreateBiomesHandler: ICommandHandler<CmdCreateBiomes>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateBiomesHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public bool Handle(CmdCreateBiomes command)
    {
        var VoronoiNose = new Voronoi(command.CountPoints, command.Width, command.Height);
        VoronoiNose.GenerateNose();
        VoronoiNose.LloydRelaxation(command.LloydRelaxations);

        World world = _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId);
        int width = world.Origin.width;
        int height = world.Origin.height;

        for (int i = 0; i < VoronoiNose.Polygons.Count; i++)
        {
            List<int[]> points = new();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (VoronoiNose.Polygons.ElementAt(i).CheckPointInsidePolygon(new Vector2(x, y)))
                    {
                        points.Add(new int[2]{x, y});
                    }
                }
            }
            world.BiomeAdd(new BiomeData(i, VoronoiNose.Polygons.ElementAt(i), points));
        }
        return true;
    }
}
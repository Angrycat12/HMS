using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DelaunayVoronoi;
using UnityEngine;

public class CmdCreateBiomesHandler : ICommandHandlerAsync<CmdCreateBiomes>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateBiomesHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public async Task<bool> Handle(CmdCreateBiomes command, CancellationToken token)
    {
        List<BiomeData> biomes = await Task.Run(() => NewMetod(command), token);
        World world = _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId);
        biomes.ForEach(b => world.BiomeAdd(b));
        return true;
    }

    private List<BiomeData> NewMetod(CmdCreateBiomes command)
    {
        List<BiomeData> biomes = new();
        var VoronoiNose = new Voronoi(command.CountPoints, command.Width, command.Height);
        VoronoiNose.GenerateNose();
        VoronoiNose.LloydRelaxation(command.LloydRelaxations);

        int width = command.Width;
        int height = command.Height;

        for (int i = 0; i < VoronoiNose.Polygons.Count; i++)
        {
            List<int[]> points = new();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (VoronoiNose.Polygons.ElementAt(i).CheckPointInsidePolygon(new Vector2(x, y)))
                    {
                        points.Add(new int[2] { x, y });
                    }
                }
            }
            biomes.Add(new BiomeData(i, VoronoiNose.Polygons.ElementAt(i), points));
        }

        return biomes;
    }
}
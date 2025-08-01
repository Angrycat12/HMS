using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Noise;
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
        var points = GetPoints(command.Width, command.Height, command.CountPoints);
        List<BiomeData> biomes = await Task.Run(() => NewMetod(command, points), token);
        // List<BiomeData> biomes = NewMetod(command, points);
        World world = _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId);
        biomes.ForEach(b => world.BiomeAdd(b));
        return true;
    }

    private List<Point> GetPoints(int width, int height, int countPoints)
    {
        List<Point> points = new();
        for (int i = 0; i < countPoints; i++)
        {
            points.Add(new(Random.Range((float)0.1, (float)(width - 0.1)), Random.Range((float)0.1, (float)(height - 0.1))));
        }
        return points;
    }

    private List<BiomeData> NewMetod(CmdCreateBiomes command, List<Point> pointsB)
    {
        List<BiomeData> biomes = new();

        var VoronoiNose = Voronoi.GenerateNoise(pointsB, command.Width, command.Height, command.LloydRelaxations);

        int width = command.Width;
        int height = command.Height;

        for (int i = 0; i < VoronoiNose.Count; i++)
        {
            List<int[]> points = new();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (VoronoiNose.ElementAt(i).CheckPointInsidePolygon(new Point(x, y)))
                    {
                        points.Add(new int[2] { x, y });
                    }
                }
            }
            biomes.Add(new BiomeData(i, VoronoiNose.ElementAt(i), points));
        }

        return biomes;
    }
}
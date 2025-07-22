using System;
using System.Linq;
using UnityEngine;

class CmdCreateCityHandler : ICommandHandler<CmdCreateCity>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateCityHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public bool Handle(CmdCreateCity command)
    {
        World world = _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId);

        for (int i = 0; i < command.Count; i++)
        {
            Vector3 maxPointCord = new();
            float maxPointValue = 0;

            for (int x = 0; x < command.Width; x++)
            {
                for (int y = 0; y < command.Height; y++)
                {
                    if (command.QualityOfCityLocations[x, y] == 0)
                    {
                        continue;
                    }

                    if (command.QualityOfCityLocations[x, y] > maxPointValue)
                    {
                        foreach (City city in world.Cities)
                        {
                            if (50 <= Math.Abs(city.Origin.position.x - x) && 50 <= Math.Abs(city.Origin.position.y - y))
                            {
                                maxPointValue = command.QualityOfCityLocations[x, y];
                                maxPointCord.x = x;
                                maxPointCord.y = y;
                            }
                        }
                    }
                }
            }

            world.Cities.Add(new City(new CityData()
            {
                position = maxPointCord
            }));

        }
        return true;
    }
}
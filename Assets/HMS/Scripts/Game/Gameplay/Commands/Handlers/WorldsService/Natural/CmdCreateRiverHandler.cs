using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class CmdCreateRiverHandler : ICommandHandler<CmdCreateRiver>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateRiverHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    public bool Handle(CmdCreateRiver command)
    {
        World world = _gameState.Worlds.FirstOrDefault(w => w.Origin.id == command.WorldId);

        List<Vector2Int> directions = new()
        {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(1, 0),  // Right
            new Vector2Int(-1, 0), // Left
            new Vector2Int(1, 1),  // Up-right
            new Vector2Int(-1, 1), // Up-left
            new Vector2Int(1, -1), // Down-right
            new Vector2Int(-1, -1) // Down-left
        };
        
        for (int i = 0; i < command.SourceRiver.Count; i++)
        {

            List<Vector2Int> river = new()
            {
                new Vector2Int(command.SourceRiver[i].x, command.SourceRiver[i].y) // Start
            };

            bool breakGenerateRiver = false;

            while (!breakGenerateRiver)
            {
                Vector2Int center = river.Last();

                Vector2Int minPointCord = center;
                float minValue = 1;

                for (int p = 0; p < directions.Count; p++)
                {
                    Vector2Int nextPoint = center + directions[p];
                    float nextPointValue = command.HeightMap[nextPoint.x, nextPoint.y];

                    if (nextPointValue <= command.WaterLevel)
                    {
                        breakGenerateRiver = true;
                        break;
                    }

                    if (nextPointValue < minValue)
                    {
                        minPointCord = nextPoint;
                        minValue = nextPointValue;
                    }
                }

                if (minValue > command.HeightMap[center.x, center.y]) break; // возможно потом создать тут функционал генерации озер

                if (!breakGenerateRiver) river.Add(minPointCord);
            }
            world.Rivers.Add(new River(new RiverData(river)));
        }
        return true;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CmdCreateRoadHandler : ICommandHandler<CmdCreateRoad>
{
    private readonly GameStateProxy _gameState;

    public CmdCreateRoadHandler(GameStateProxy gameState)
    {
        _gameState = gameState;
    }

    static void Swap<T>(ref T x, ref T y)
    {
        T t = y;
        y = x;
        x = t;
    }

    public bool Handle(CmdCreateRoad command)
    {
        List<City> Cities = SortCity(command.Cities);

        List<Road> Roads = new();
        City LastCity = Cities.ElementAt(0);

        foreach (City city in Cities)
        {
            if (city == LastCity) continue;

            Vector2Int Start = LastCity.Origin.position2;
            Vector2Int Stop = city.Origin.position2;

            List<Vector2> points = new()
            {
                Start
            };

            bool IsNotGoodRoad = false;
            
            foreach (var point in BresenhamLine(Start, Stop))
            {
                if (command.WaterLevel >= command.HeightMap[point.x, point.y]) IsNotGoodRoad = true;
            }

            points.Add(Stop);

            if (!IsNotGoodRoad) Roads.Add(new(new RoadData() { PointId1 = LastCity.Origin.id, PointId2 = city.Origin.id, Points = points }));

            LastCity = city;
        }

        return true;
    }

    private List<City> SortCity(List<City> cities)
    {
        List<City> sortedCities = new()
        {
            cities[0]
        };

        City nextCity = cities.Last();
        Vector2 DistanceNextCity = new(Distance(nextCity.Origin.position3.x, sortedCities.Last().Origin.position3.x),
                                    Distance(nextCity.Origin.position3.y, sortedCities.Last().Origin.position3.y));

        for (int i = 0; i < cities.Count; i++)
        {
            foreach (City city in cities)
            {
                if (sortedCities.Contains(city)) continue;

                Vector2 distance = new(Distance(city.Origin.position3.x, sortedCities.Last().Origin.position3.x),
                                        Distance(city.Origin.position3.y, sortedCities.Last().Origin.position3.y));

                if (distance.x <= DistanceNextCity.x && distance.y < DistanceNextCity.y ||
                    distance.x < DistanceNextCity.x && distance.y <= DistanceNextCity.y)
                {
                    nextCity = city;
                    DistanceNextCity = distance;

                }
            }
            sortedCities.Add(nextCity);
        }

        return sortedCities;
    }

    private float Distance(float a, float b)
    {
        return Math.Abs(a - b);
    }

    private List<Vector2Int> BresenhamLine(Vector2Int Start, Vector2Int Stop)
    {
        List<Vector2Int> answer = new();
        int x0 = Start.x;
        int x1 = Stop.x;
        int y0 = Start.y;
        int y1 = Stop.y;
        var steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0); // Проверяем рост отрезка по оси икс и по оси игрек
        // Отражаем линию по диагонали, если угол наклона слишком большой
        if (steep)
        {
            Swap(ref x0, ref y0); // Перетасовка координат вынесена в отдельную функцию для красоты
            Swap(ref x1, ref y1);
        }
        // Если линия растёт не слева направо, то меняем начало и конец отрезка местами
        if (x0 > x1)
        {
            Swap(ref x0, ref x1);
            Swap(ref y0, ref y1);
        }
        int dx = x1 - x0;
        int dy = Math.Abs(y1 - y0);
        int error = dx / 2; // Здесь используется оптимизация с умножением на dx, чтобы избавиться от лишних дробей
        int ystep = (y0 < y1) ? 1 : -1; // Выбираем направление роста координаты y
        int y = y0;
        for (int x = x0; x <= x1; x++)
        {
            answer.Add(new(steep ? y : x, steep ? x : y)); // Не забываем вернуть координаты на место
            error -= dy;
            if (error < 0)
            {
                y += ystep;
                error += dx;
            }
        }
        return answer;
        // https://habr.com/ru/articles/185086/
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Angrycat.Noise
{
    public static class Delaunay
    {
        /// <summary>
        /// Генерирует триангуляцию Делоне для заданного набора точек, используя алгоритм Бойера-Ватсона.
        /// </summary>
        /// <param name="points">Список точек для триангуляции.</param>
        /// <returns>Список треугольников, образующих триангуляцию Делоне.</returns>
        public static List<DelaunayTriangle> GenerateNoise(List<Point> points)
        {
            if (points == null || points.Count < 3)
            {
                throw new ArgumentException("Для триангуляции требуется как минимум 3 точки.");
            }

            // 1. Создание "супер-треугольника", который охватывает все точки
            double minX = points.Min(p => p.x);
            double minY = points.Min(p => p.y);
            double maxX = points.Max(p => p.x);
            double maxY = points.Max(p => p.y);

            double width = maxX - minX;
            double height = maxY - minY;
            double maxDim = Math.Max(width, height);

            // Надежный способ построения супер-треугольника
            // Используем большой буфер, чтобы гарантировать, что все точки находятся внутри
            double buffer = maxDim > 0 ? maxDim * 2 : 1000; // Если maxDim 0 (одна точка или все в одной), используем дефолтный буфер
            if (buffer < 100) buffer = 100; // Гарантируем минимальный буфер

            Point superP1 = new Point(minX - buffer, minY - buffer);
            Point superP2 = new Point(maxX + buffer, minY - buffer);
            Point superP3 = new Point(minX + width / 2, maxY + buffer);

            // Инициализируем триангуляцию супер-треугольником
            List<DelaunayTriangle> triangulation = new List<DelaunayTriangle> { new DelaunayTriangle(superP1, superP2, superP3) };

            // 2. Добавление каждой точки по одной
            foreach (Point point in points)
            {
                List<DelaunayTriangle> badTriangles = new List<DelaunayTriangle>();
                foreach (DelaunayTriangle triangle in triangulation)
                {
                    if (triangle.ContainsInCircumcircle(point))
                    {
                        badTriangles.Add(triangle);
                    }
                }

                List<Edge> polygon = new List<Edge>();
                // Находим границы многоугольного отверстия
                foreach (DelaunayTriangle badTriangle in badTriangles)
                {
                    Edge[] edges = { badTriangle.Edges[0], badTriangle.Edges[1], badTriangle.Edges[2] };
                    foreach (Edge edge in edges)
                    {
                        // Если ребро не является общим для другого "плохого" треугольника, оно является частью границы
                        bool isShared = false;
                        foreach (DelaunayTriangle otherBadTriangle in badTriangles)
                        {
                            if (otherBadTriangle != badTriangle && otherBadTriangle.Equals(badTriangle) == false) // Проверяем, что это не тот же треугольник
                            {
                                if (otherBadTriangle.Edges[0].Equals(edge) || otherBadTriangle.Edges[1].Equals(edge) || otherBadTriangle.Edges[2].Equals(edge))
                                {
                                    isShared = true;
                                    break;
                                }
                            }
                        }
                        if (!isShared)
                        {
                            polygon.Add(edge);
                        }
                    }
                }

                // Удаляем "плохие" треугольники из триангуляции
                triangulation.RemoveAll(t => badTriangles.Contains(t));

                // Ре-триангулируем многоугольное отверстие
                foreach (Edge edge in polygon)
                {
                    triangulation.Add(new DelaunayTriangle(edge.Vertex[0], edge.Vertex[1], point));
                }
            }

            // 3. Удаляем любые треугольники, которые используют вершины исходного супер-треугольника
            List<DelaunayTriangle> finalDelaunayTriangles = new List<DelaunayTriangle>();
            foreach (DelaunayTriangle Delaunaytriangle in triangulation)
            {
                if (!Delaunaytriangle.HasSuperTrianglePoint(superP1, superP2, superP3))
                {
                    finalDelaunayTriangles.Add(Delaunaytriangle);
                }
            }

            return finalDelaunayTriangles;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class DelaunayToVoronoi
    {
        public HashSet<Edge> voronoiEdges       { get; private set; }
        public HashSet<Polygon> voronoiPolygons { get; private set; }

        public void GenerateEdgesFromDelaunay(HashSet<Triangle> triangulation)
        {
            voronoiEdges = new HashSet<Edge>();
            foreach (var triangle in triangulation)
            {
                foreach (var neighbor in triangle.TrianglesWithSharedEdge)
                {
                    var edge = new Edge(triangle.Circumcenter, neighbor.Circumcenter);
                    voronoiEdges.Add(edge);
                }
            }
        }

        private bool Crossed(Point point, float Width, float Height)
        {
            return (0 < point.X && point.X < Height) && (0 < point.Y && point.Y < Width);
        }

        public void GeneratePolygonFromDelaunay(HashSet<Point> points, float Width, float Height)
        {
            voronoiPolygons = new HashSet<Polygon>();

            foreach (var point in points)
            {
                var circumcenters = new HashSet<Point>();

                foreach (var triangle in point.AdjacentTriangles)
                {
                    circumcenters.Add(triangle.Circumcenter);
                }

                if (circumcenters.Count < 3) continue; // Пропускаем, если не хватает точек для создания многоугольника

                // Находим центр многоугольника (усреднение всех циркумцентров)
                var center = new Point(
                    circumcenters.Average(p => p.X),
                    circumcenters.Average(p => p.Y)
                );

                // Сортируем циркумцентры по углу относительно центра
                var sortedCircumcenters = circumcenters
                    .OrderBy(p => Mathf.Atan2(p.Y - center.Y, p.X - center.X))
                    .ToList();

                var polygonEdges = new HashSet<Edge>();
                var polygonVertices = new HashSet<Vector2>();

                int count = sortedCircumcenters.Count;

                for (int i = 0; i < count; i++)
                {
                    var start = sortedCircumcenters[i];
                    var end = sortedCircumcenters[(i + 1) % count];

                    // Ограничиваем грани границами области
                    if (Crossed(start, Width, Height) && Crossed(end, Width, Height))
                    {
                        polygonEdges.Add(new Edge(start, end));
                        polygonVertices.Add(start.ToVector2());
                        polygonVertices.Add(end.ToVector2());
                    }
                    else
                    {
                        // Обрабатываем случай, когда одна из точек выходит за границы
                        var clippedEdge = ClipEdgeToBounds(start, end, Width, Height);
                        if (clippedEdge != null)
                        {
                            polygonEdges.Add(clippedEdge);
                            polygonVertices.Add(clippedEdge.Point1.ToVector2());
                            polygonVertices.Add(clippedEdge.Point2.ToVector2());
                        }
                    }
                }

                voronoiPolygons.Add(new Polygon(polygonEdges.ToList(),polygonVertices.ToArray(), point));
            }

            // Find neighbours
            foreach (Polygon polygon in voronoiPolygons)
            {
                List<Point> NeighbourCenterPoint = new();
                foreach(Triangle triangle in polygon.centerpoint.AdjacentTriangles)
                {
                    foreach (var point in triangle.Vertices)
                    {
                        if (point != polygon.centerpoint)
                        NeighbourCenterPoint.Add(point);
                    }
                }

                List<Polygon> PolygonsNeighbour = new();
                foreach (var npolygon in voronoiPolygons)
                {
                    if (NeighbourCenterPoint.Contains(npolygon.centerpoint))
                        PolygonsNeighbour.Add(npolygon);
                }
                polygon.AddNeighbour(PolygonsNeighbour);
            }
        }

        private Edge ClipEdgeToBounds(Point start, Point end, float Width, float Height)
        {
            float minX = 0f;
            float maxX = Width;
            float minY = 0f;
            float maxY = Height;

            float x1 = start.X;
            float y1 = start.Y;
            float x2 = end.X;
            float y2 = end.Y;

            int code1 = ComputeOutCode(x1, y1, minX, maxX, minY, maxY);
            int code2 = ComputeOutCode(x2, y2, minX, maxX, minY, maxY);

            bool accept = false;

            while (true)
            {
                if ((code1 | code2) == 0)
                {
                    accept = true;
                    break;
                }
                else if ((code1 & code2) != 0)
                {
                    break;
                }
                else
                {
                    float x = 0, y = 0;
                    int outcodeOut = (code1 != 0) ? code1 : code2;

                    if ((outcodeOut & 8) != 0)
                    {
                        x = x1 + (x2 - x1) * (maxY - y1) / (y2 - y1);
                        y = maxY;
                    }
                    else if ((outcodeOut & 4) != 0)
                    {
                        x = x1 + (x2 - x1) * (minY - y1) / (y2 - y1);
                        y = minY;
                    }
                    else if ((outcodeOut & 2) != 0)
                    {
                        y = y1 + (y2 - y1) * (maxX - x1) / (x2 - x1);
                        x = maxX;
                    }
                    else if ((outcodeOut & 1) != 0)
                    {
                        y = y1 + (y2 - y1) * (minX - x1) / (x2 - x1);
                        x = minX;
                    }

                    if (outcodeOut == code1)
                    {
                        x1 = x;
                        y1 = y;
                        code1 = ComputeOutCode(x1, y1, minX, maxX, minY, maxY);
                    }
                    else
                    {
                        x2 = x;
                        y2 = y;
                        code2 = ComputeOutCode(x2, y2, minX, maxX, minY, maxY);
                    }
                }
            }

            if (accept)
            {
                return new Edge(new Point(x1, y1), new Point(x2, y2));
            }
            else
            {
                return null;
            }
        }

        private int ComputeOutCode(float x, float y, float minX, float maxX, float minY, float maxY)
        {
            int code = 0;

            if (x < minX) code |= 1; // Левее границы
            else if (x > maxX) code |= 2; // Правее границы
            if (y < minY) code |= 4; // Ниже границы
            else if (y > maxY) code |= 8; // Выше границы

            return code;
        }
    }
}

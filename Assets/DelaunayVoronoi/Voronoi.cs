using System.Collections.Generic;

namespace DelaunayVoronoi
{
    public class Voronoi
    {
        public int PointCount = 2000;
        public int Width { get; }
        public int Height { get; }
        public HashSet<Polygon> Polygons { get; set; }
        public HashSet<Triangle> triangulation;
        public IEnumerable<Edge> Edges;
        private DelaunayTriangulator delaunay;
        private DelaunayToVoronoi voronoi = new();

        public Voronoi(int PointCount, int Width, int Height)
        {
            this.PointCount = PointCount;
            this.Width = Width;
            this.Height = Height;
        }

        public void GenerateNose()
        {
            delaunay = new DelaunayTriangulator();
            var points = delaunay.GeneratePoints(PointCount, Width, Height);
            GenerateVoronoi(points);
        }

        // Основной метод для генерации диаграммы Вороного
        public void GenerateVoronoi(HashSet<Point> points)
        {
            triangulation = delaunay.BowyerWatson(points);
            voronoi.GenerateEdgesFromDelaunay(triangulation);
            voronoi.GeneratePolygonFromDelaunay(points, Width, Height);
            Edges = voronoi.voronoiEdges;
            Polygons = voronoi.voronoiPolygons;
        }

        // Метод для релаксации Ллойда
        public void LloydRelaxation(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var newPoints = new HashSet<Point>();

                // Перемещаем каждую точку к центроиду ее многоугольника
                foreach (var polygon in Polygons)
                {
                    var centroid = CalculateCentroid(polygon);

                    // Убедитесь, что новый центроид находится в пределах границ области
                    if (centroid.X >= 0 && centroid.X <= Width && centroid.Y >= 0 && centroid.Y <= Height)
                    {
                        newPoints.Add(centroid);
                    }
                }

                // Заново генерируем Вороного с новыми точками
                GenerateVoronoi(newPoints);
            }
        }

        // Метод для вычисления центра многоугольника Вороного
        private Point CalculateCentroid(Polygon polygon)
        {
            float xSum = 0;
            float ySum = 0;
            int pointCount = 0;

            foreach (var edge in polygon.edges)
            {
                xSum += edge.Point1.X;
                ySum += edge.Point1.Y;
                pointCount++;

                xSum += edge.Point2.X;
                ySum += edge.Point2.Y;
                pointCount++;
            }

            return new Point(xSum / pointCount, ySum / pointCount);
        }
    }
}

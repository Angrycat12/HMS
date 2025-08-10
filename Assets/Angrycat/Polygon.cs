using System;
using System.Collections.Generic;
using System.Linq;

namespace Angrycat
{
    public class Polygon
    {
        public Point[] Vertex { get; protected set; }
        public Point Centroid { get; protected set; }
        public Edge[] Edges { get; protected set; }

        protected Polygon()
        {}

        public Polygon(Point[] points)
        {
            if (points.Count() <= 2)
            {
                throw new ArgumentException("Must be 3 or more points");
            }

            Vertex = points;
            GetCentroid();
            BuildConvexHull();
            GetEdges();
        }

        public Polygon(List<Point> points) : this(points.ToArray()) { }

        public Polygon(Edge[] edges)
        {
            if (edges.Count() <= 2)
            {
                throw new ArgumentException("Must be 3 or more edges");
            }

            Vertex = new Point[edges.Count()];
            for (int i = 0; i < edges.Count(); i++)
            {
                Vertex[i] = edges[i].Vertex[1];
            }

            GetCentroid();
            // MercatorСhain();
            GetEdges();
        }

        public Polygon GetPolygon() => this;

        public bool CheckPointInsidePolygon(Point point)
        {
            List<Point> vertices = Vertex.ToList();

            int crossings = 0;

            for (int i = 0, j = vertices.Count - 1; i < vertices.Count; j = i++)
            {
                var v1 = vertices[i];
                var v2 = vertices[j];

                if (((v1.y > point.y) != (v2.y > point.y)) &&
                    (point.x < (v2.x - v1.x) * (point.y - v1.y) / (v2.y - v1.y) + v1.x))
                {
                    crossings++;
                }
            }

            // Если количество пересечений нечетное, точка внутри многоугольника
            return crossings % 2 != 0;
        }

        public HashSet<Triangle> Triangulate()
        {
            var triangles = new HashSet<Triangle>();
            var remainingVertices = new List<Point>(Vertex);

            if (remainingVertices.Count < 3) return triangles;
            if (remainingVertices.Count == 3)
            {
                triangles.Add(new Triangle(remainingVertices[0], remainingVertices[1], remainingVertices[2]));
                return triangles;
            }

            int currentIndex = 0;
            while (remainingVertices.Count > 3)
            {
                int prevIndex = (currentIndex == 0) ? remainingVertices.Count - 1 : currentIndex - 1;
                int nextIndex = (currentIndex + 1) % remainingVertices.Count;

                Point p_prev = remainingVertices[prevIndex];
                Point p_curr = remainingVertices[currentIndex];
                Point p_next = remainingVertices[nextIndex];
                
                // Проверяем, является ли текущая вершина "ухом"
                if (IsEar(p_prev, p_curr, p_next, remainingVertices))
                {
                    triangles.Add(new Triangle(p_prev, p_curr, p_next));
                    remainingVertices.RemoveAt(currentIndex);
                    // Сбрасываем индекс для начала нового поиска
                    currentIndex = 0; 
                }
                else
                {
                    currentIndex++;
                    if (currentIndex >= remainingVertices.Count)
                    {
                        // Если мы прошли весь полигон и не нашли ухо, это может означать,
                        // что полигон слишком сложный или имеет самопересечения.
                        // В данном случае просто выходим, чтобы избежать бесконечного цикла.
                        break; 
                    }
                }
            }

            // Добавляем последний оставшийся треугольник
            if (remainingVertices.Count == 3)
            {
                triangles.Add(new Triangle(remainingVertices[0], remainingVertices[1], remainingVertices[2]));
            }

            return triangles;
        }

        private bool IsEar(Point p1, Point p2, Point p3, List<Point> polygonVertices)
        {
            // Ухо должно быть выпуклой вершиной (для CCW полигона кросс-продукт > 0)
            if (CrossProduct(p1, p2, p3) < 0)
                return false;

            // Внутри треугольника-уха не должно быть других вершин полигона
            Triangle potentialEar = new(p1, p2, p3);
            foreach (var p in polygonVertices)
            {
                if (p.Equals(p1) || p.Equals(p2) || p.Equals(p3))
                    continue;

                // Используем проверку на нахождение точки внутри треугольника
                if (IsPointInTriangle(p, p1, p2, p3))
                    return false;
            }
            return true;
        }

        private bool IsPointInTriangle(Point pt, Point v1, Point v2, Point v3)
        {
            double d1 = CrossProduct(pt, v1, v2);
            double d2 = CrossProduct(pt, v2, v3);
            double d3 = CrossProduct(pt, v3, v1);
            
            bool has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        
        private static double CrossProduct(Point p, Point q, Point r)
        {
            // (q.X - p.X) * (r.Y - p.Y) - (q.Y - p.Y) * (r.X - p.X)
            return (q.x - p.x) * (r.y - p.y) - (q.y - p.y) * (r.x - p.x);
        }

        private void BuildConvexHull()
        {
            List<Point> points = Vertex.ToList();

            if (points == null || points.Count <= 2)
            {
                // Для построения оболочки нужно хотя бы 3 неколлинеарные точки.
                // Если 0, 1 или 2 точки, то оболочка - это сами точки.
                return;
            }

            // 1. Сортируем точки по X, затем по Y.
            // Это гарантирует, что мы начинаем с самой левой нижней точки и движемся вправо.
            points = points.OrderBy(p => p.x)
                        .ThenBy(p => p.y)
                        .ToList();

            List<Point> hull = new List<Point>();

            // 2. Построение нижней цепочки оболочки
            foreach (var p in points)
            {
                // Пока в hull есть хотя бы 2 точки и последние 3 точки делают "правый поворот" (или коллинеарны)
                // (т.е. новая точка P будет внутри или на отрезке, образованном двумя предыдущими, если смотреть с внешней стороны)
                while (hull.Count >= 2 && CrossProduct(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                {
                    hull.RemoveAt(hull.Count - 1); // Удаляем последнюю точку из hull
                }
                hull.Add(p); // Добавляем текущую точку
            }

            // 3. Построение верхней цепочки оболочки
            // Запоминаем размер нижней цепочки, чтобы не обрабатывать ее заново
            int lowerHullSize = hull.Count;

            // Проходим по отсортированным точкам в обратном порядке, исключая последнюю точку (которая будет первой точкой верхней цепочки)
            for (int i = points.Count - 2; i >= 0; i--)
            {
                var p = points[i];
                // Пока в hull есть хотя бы точки из верхней цепочки (после нижней)
                // и последние 3 точки делают "правый поворот" (или коллинеарны)
                while (hull.Count > lowerHullSize && CrossProduct(hull[hull.Count - 2], hull[hull.Count - 1], p) <= 0)
                {
                    hull.RemoveAt(hull.Count - 1); // Удаляем последнюю точку из hull
                }
                hull.Add(p); // Добавляем текущую точку
            }

            // 4. Удаление дублирующейся начальной/конечной точки
            // Последняя точка в hull будет дублировать первую, поэтому удаляем ее.
            // Если точек меньше 2, это означает, что все точки были коллинеарны или их было мало.
            if (hull.Count > 1)
            {
                hull.RemoveAt(hull.Count - 1);
            }

            Vertex = hull.ToArray();
        }

        private void GetVertex()
        {
            throw new ArgumentException("void polygon");
        }

        private void GetCentroid()
        {
            if (Vertex is null) throw new ArgumentException("void polygon");

            // Obtaining the centroid of irregular convex polygons
            // https://www.baeldung.com/cs/visual-center-polygon
            double x = 0;
            double y = 0;

            for (int i = 0; i < Vertex.Count(); i++)
            {
                x += Vertex[i].x;
                y += Vertex[i].y;
            }

            Centroid = new Point(x / Vertex.Count(), y / Vertex.Count());
        }

        private void GetEdges()
        {
            if (Vertex is null) throw new ArgumentException("void polygon");

            Edges = new Edge[Vertex.Count()];

            for (int i = 1; i < Vertex.Count(); i++)
            {
                Edges[i - 1] = new Edge(Vertex[i - 1], Vertex[i]);
            }

            Edges[Vertex.Count() - 1] = new Edge(Vertex[Vertex.Count() - 1], Vertex[0]);
        }
    }
}
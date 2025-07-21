using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class Polygon 
    {
        public List<Edge> edges             { get; private set; }
        public Vector2[] vertices           { get; private set; }
        public Point centerpoint            { get; private set; }
        public List<Polygon> neighbours     { get; private set; }
        public HashSet<Triangle> Triangles  { get; private set; }

        public Polygon(List<Edge> edges, Vector2[] vertices, Point centerpoint) 
        {
            this.centerpoint = centerpoint;
            this.vertices = vertices;
            this.edges = edges;
            this.neighbours = new List<Polygon>();
            this.Triangles = new HashSet<Triangle>();
            //EnsurePolygonClosed();
        }

        public void AddNeighbour(List<Polygon> neighbour)
        {
            this.neighbours = neighbour;
        }

        private Point ToPoint(Vector2 value)
        {
            return new Point(value.x, value.y);
        }

        public bool CheckPointInsidePolygon(Vector2 point)
        {
            List<Vector2> vertices = this.vertices.ToList();

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

        private void EnsurePolygonClosed()
        {
            if (vertices[0] != vertices[vertices.Length - 1])
            {
                List<Vector2> closedVertices = new List<Vector2>(vertices)
                {
                    vertices[0]
                };
                vertices = closedVertices.ToArray();
            }
        }

        public HashSet<Triangle> Triangulate()
        {
            Triangles = new HashSet<Triangle>();

            // Первая вершина фиксирована
            var firstVertex = vertices[0];

            // Проходим по всем вершинам начиная со второй и третьей
            for (int i = 1; i < vertices.Count() - 1; i++)
            {
                // Создаём треугольник из первой вершины и двух последовательных вершин
                var triangle = new Triangle(ToPoint(firstVertex), ToPoint(vertices[i]), ToPoint(vertices[i + 1]));
                Triangles.Add(triangle);
            }

            return Triangles;
        }


     }
}

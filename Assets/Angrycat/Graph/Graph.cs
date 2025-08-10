using System.Collections.Generic;
using System.Linq;
using Angrycat.Noise;

namespace Angrycat.Graph
{
    public static class Graph
    {
        public static HashSet<Node> CompileNavMesh(List<DelaunayTriangle> triangulation, List<Biome> biomes)
        {
            List<Polygon> rawNavMesh = new();
            triangulation.ForEach(t => rawNavMesh.Add(t.GetPolygon()));
            var navMesh = GetNavMesh(rawNavMesh.ToHashSet());
            HashSet<Node> finishNavMash = new();

            foreach (var biome in biomes)
            {
                var neighbours = navMesh[biome.Origin.Polygon.CenterPoint];
                HashSet<Node> neighbourNodes = new();

                foreach (var neighbour in neighbours)
                {
                    Node neighbourNode = GetNodeByPoint(finishNavMash, neighbour);
                    neighbourNode ??= new(neighbour);
                    neighbourNodes.Add(neighbourNode);
                }

                Node node = GetNodeByPoint(finishNavMash, biome.Origin.Polygon.CenterPoint);

                if (node is not null) node.AddNeighbours(neighbourNodes);
                else node = new(biome.Origin.Polygon.CenterPoint, neighbourNodes);

                node.SetCost(biome.Origin.Type.QualityLandScape);
                finishNavMash.Add(node);
            }

            return finishNavMash;
        }

        private static Node GetNodeByPoint(HashSet<Node> nodes, Point point)
        {
            foreach (var node in nodes)
            {
                if (node.Position == point) return node;
            }

            return null;
        }


        private static Dictionary<Point, HashSet<Point>> GetNavMesh(HashSet<Polygon> navMesh)
        {
            Dictionary<Point, HashSet<Point>> NavMesh = new();
            foreach (var polygon in navMesh)
            {
                foreach (var vertex in polygon.Vertex)
                {
                    NavMesh = AddToNavMesh(vertex, polygon.Vertex.ToList(), NavMesh);
                }
            }

            return NavMesh;
        }

        private static Dictionary<Point, HashSet<Point>> AddToNavMesh(Point key, List<Point> neighbours, Dictionary<Point, HashSet<Point>> navMesh)
        {
            if (!navMesh.ContainsKey(key)) navMesh[key] = new();

            foreach (var neighbour in neighbours)
            {
                navMesh[key].Add(neighbour);
            }

            return navMesh;
        }
    }
}
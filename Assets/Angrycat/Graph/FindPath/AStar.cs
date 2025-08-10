using System.Collections.Generic;
using Angrycat.Collections;

namespace Angrycat.Graph
{
    public static class AStar
    {   
        /// <summary>
        /// Находит кратчайший путь в предоставленном графе от начальной до конечной позиции.
        /// </summary>
        /// <param name="graph">Весь навигационный граф в виде HashSet<Node>.</param>
        /// <param name="startPosition">Координаты начальной точки пути.</param>
        /// <param name="goalPosition">Координаты целевой точки пути.</param>
        /// <returns>Список узлов (List<Node>), представляющий путь, или пустой список, если путь не найден.</returns>
        public static List<Node> FindPath(HashSet<Node> graph, Point startPosition, Point goalPosition)
        {
            if (graph == null || graph.Count < 2)
            {
                return new List<Node>();
            }

            Node startNode = FindClosestNode(graph, startPosition);
            Node goalNode = FindClosestNode(graph, goalPosition);

            if (startNode == null || goalNode == null || startNode.Equals(goalNode))
            {
                return new List<Node>();
            }

            // УЛУЧШЕНИЕ: Использование PriorityQueue вместо List для openSet.
            // Это изменяет сложность извлечения лучшего узла с O(N) до O(log N).
            var openSet = new PriorityQueue<Node, double>();
            
            var cameFrom = new Dictionary<Node, Node>();
            var gScore = new Dictionary<Node, double>();

            // Инициализируем gScore для всех узлов значением "бесконечность".
            foreach (var node in graph)
            {
                gScore[node] = double.MaxValue;
            }
            gScore[startNode] = 0;

            openSet.Enqueue(startNode, Heuristic(startNode, goalNode));

            while (openSet.Count > 0)
            {
                // УЛУЧШЕНИЕ: Dequeue извлекает элемент с наименьшим приоритетом за O(log N).
                var current = openSet.Dequeue();

                if (current.Equals(goalNode))
                {
                    return ReconstructPath(cameFrom, current);
                }

                if (current.Neighbours == null) continue;

                foreach (var neighbour in current.Neighbours)
                {
                    var tentative_gScore = gScore[current] 
                                           + (current.Position - neighbour.Position).Length() 
                                           + neighbour.Cost;

                    if (tentative_gScore < gScore.GetValueOrDefault(neighbour, double.MaxValue))
                    {
                        cameFrom[neighbour] = current;
                        gScore[neighbour] = tentative_gScore;
                        double fScore = tentative_gScore + Heuristic(neighbour, goalNode);
                        
                        // Просто добавляем узел в очередь. PriorityQueue сама справится с дубликатами,
                        // и мы всегда сначала обработаем узел с наименьшей стоимостью.
                        openSet.Enqueue(neighbour, fScore);
                    }
                }
            }
            
            return new List<Node>(); // Путь не найден
        }
        
        private static Node FindClosestNode(HashSet<Node> graph, Point position)
        {
            Node closestNode = null;
            double minDistanceSq = double.MaxValue;

            foreach (var node in graph)
            {
                double distSq = (node.Position - position).LengthSquared();
                if (distSq < minDistanceSq)
                {
                    minDistanceSq = distSq;
                    closestNode = node;
                }
            }
            return closestNode;
        }

        private static double Heuristic(Node node, Node goal)
        {
            return (node.Position - goal.Position).Length();
        }

        private static List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
        {
            var totalPath = new List<Node> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            return totalPath;
        }
    }
}
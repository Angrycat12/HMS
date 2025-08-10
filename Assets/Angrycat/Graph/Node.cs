using System;
using System.Collections.Generic;

namespace Angrycat.Graph
{
    public class Node
    {
        public readonly Point Position;
        public double Cost { get; private set; }
        public double Quality { get; private set; }
        public HashSet<Node> Neighbours { get; private set; }

        public Node(Point position, HashSet<Node> neighbours)
        {
            Position = position;
            Neighbours = neighbours;
        }

        public Node(Point position)
        {
            Position = position;
        }

        public void SetCost(double cost) => Cost = cost;

        public void AddNeighbours(HashSet<Node> neighbours)
        {
            Neighbours.UnionWith(neighbours);
        }

        public override bool Equals(object obj)
        {
            if (obj is Node other)
            {
                double epsilon = 1e-9; // Для сравнения с плавающей точкой
                return Math.Abs(Position.x - other.Position.x) < epsilon && Math.Abs(Position.y - other.Position.y) < epsilon;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Используем HashCode.Combine для надежного хэша
            return HashCode.Combine(Position);
        }
    }
}
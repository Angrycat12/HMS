using System.Collections.Generic;
using UnityEngine;

namespace DelaunayVoronoi
{
    public class Point
    {
        /// <summary>
        /// Used only for generating a unique ID for each instance of this class that gets generated
        /// </summary>
        private static int _counter;

        /// <summary>
        /// Used for identifying an instance of a class; can be useful in troubleshooting when geometry goes weird
        /// (e.g. when trying to identify when Triangle objects are being created with the same Point object twice)
        /// </summary>
        private readonly int _instanceId = _counter++;

        public float X { get; }
        public float Y { get; }
        public HashSet<Triangle> AdjacentTriangles { get; } = new HashSet<Triangle>();

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2((float)X, (float)Y);
        }

        public Vector2 ToVector3()
        {
            return new Vector3((float)X, (float)Y, 0);
        }

        public override string ToString()
        {
            // Simple way of seeing what's going on in the debugger when investigating weirdness
            return $"{nameof(Point)} {_instanceId} {X:0.##}@{Y:0.##}";
        }
    }
}
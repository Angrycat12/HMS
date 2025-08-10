using System;
using UnityEngine;

namespace Angrycat
{
    public class Point
    {
        public readonly double x;
        public readonly double y;

        public Point(double X, double Y)
        {
            x = X;
            y = Y;
        }

        public Vector2 ToVector2()
        {
            return new((float)x, (float)y);
        }

        public Vector3 ToVector3()
        {
            return new((float)x, (float)y);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point other)
            {
                double epsilon = 1e-9; // Для сравнения с плавающей точкой
                return Math.Abs(x - other.x) < epsilon && Math.Abs(y - other.y) < epsilon;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Используем HashCode.Combine для надежного хэша
            return HashCode.Combine(x, y);
        }

        public override string ToString() => $"({x} ,{y} )";
        
        public static Point operator +(Point p1, Point p2) => new Point(p1.x + p2.x, p1.y + p2.y);
        public static Point operator -(Point p1, Point p2) => new Point(p1.x - p2.x, p1.y - p2.y);
        public static Point operator *(Point p, double scalar) => new Point(p.x * scalar, p.y * scalar);
        public static Point operator /(Point p, double scalar)
        {
            if (scalar == 0) throw new DivideByZeroException("Деление на ноль в операторе Point / double.");
            return new Point(p.x / scalar, p.y / scalar);
        }

        public double LengthSquared() => x * x + y * y;
        public double Length() => Math.Sqrt(LengthSquared());
    }
}
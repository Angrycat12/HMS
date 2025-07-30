using System;

namespace Noise
{
    public class Triangle : Polygon
    {
        public new Point[] Vertex { get; } = new Point[3];
        public Point Circumcenter { get; private set; }
        public double RadiusSquared;
        public new Edge[] Edges = new Edge[3];

        public Point P1 => Vertex[0];
        public Point P2 => Vertex[0];
        public Point P3 => Vertex[0];

        public Edge E12 => Edges[0];
        public Edge E23 => Edges[1];
        public Edge E31 => Edges[2];


        public Triangle(Point point1, Point point2, Point point3)
        {
            // In theory this shouldn't happen, but it was at one point so this at least makes sure we're getting a
            // relatively easily-recognised error message, and provides a handy breakpoint for debugging.
            if (point1 == point2 || point1 == point3 || point2 == point3)
            {
                throw new ArgumentException("Must be 3 distinct points");
            }

            if (!IsCounterClockwise(point1, point2, point3))
            {
                Vertex[0] = point1; Edges[0] = new(point1, point3);
                Vertex[1] = point3; Edges[1] = new(point3, point2);
                Vertex[2] = point2; Edges[2] = new(point2, point1);
            }
            else
            {
                Vertex[0] = point1; Edges[0] = new(point1, point2);
                Vertex[1] = point2; Edges[1] = new(point2, point3);
                Vertex[2] = point3; Edges[2] = new(point3, point1);
            }

            // Vertex[0].AdjacentTriangles.Add(this);
            // Vertex[1].AdjacentTriangles.Add(this);
            // Vertex[2].AdjacentTriangles.Add(this);

            UpdateCircumcircle();
        }

        public bool IsPointInsideCircumcircle(Point point)
        {
            // Проверка на очень маленький или вырожденный радиус, чтобы избежать ошибок
            if (double.IsNaN(Circumcenter.x) || double.IsNaN(Circumcenter.y) || double.IsInfinity(RadiusSquared))
            {
                return false;
            }

            var d_squared = (point.x - Circumcenter.x) * (point.x - Circumcenter.x) +
                (point.y - Circumcenter.y) * (point.y - Circumcenter.y);
            
            // Используем небольшой эпсилон для устойчивости сравнения
            double epsilon = 1e-9;
            return d_squared < RadiusSquared - epsilon;
        }

        private void UpdateCircumcircle()
        {
            // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
            // https://en.wikipedia.org/wiki/Circumscribed_circle
            var p0 = Vertex[0];
            var p1 = Vertex[1];
            var p2 = Vertex[2];
            var dA = p0.x * p0.x + p0.y * p0.y;
            var dB = p1.x * p1.x + p1.y * p1.y;
            var dC = p2.x * p2.x + p2.y * p2.y;

            var aux1 = (dA * (p2.y - p1.y) + dB * (p0.y - p2.y) + dC * (p1.y - p0.y));
            var aux2 = -(dA * (p2.x - p1.x) + dB * (p0.x - p2.x) + dC * (p1.x - p0.x));
            var div = (2 * (p0.x * (p2.y - p1.y) + p1.x * (p0.y - p2.y) + p2.x * (p1.y - p0.y)));

            if (div == 0)
            {
                throw new DivideByZeroException();
            }

            var center = new Point(aux1 / div, aux2 / div);
            Circumcenter = center;
            RadiusSquared = (center.x - p0.x) * (center.x - p0.x) + (center.y - p0.y) * (center.y - p0.y);
        }
        
        private bool IsCounterClockwise(Point point1, Point point2, Point point3)
        {
            var result = (point2.x - point1.x) * (point3.y - point1.y) -
                (point3.x - point1.x) * (point2.y - point1.y);
            return result > 0;
        }
    }
}
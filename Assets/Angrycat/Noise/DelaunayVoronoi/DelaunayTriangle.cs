namespace Angrycat.Noise
{
    public class DelaunayTriangle : Triangle
    {
        public DelaunayTriangle(Point point1, Point point2, Point point3) : base(point1, point2, point3)
        {

        }

        public bool ContainsInCircumcircle(Point point)
        {

            return base.IsPointInsideCircumcircle(point);

            // double ax = Vertex[0].x, ay = Vertex[0].y;
            // double bx = Vertex[1].x, by = Vertex[1].y;
            // double cx = Vertex[2].x, cy = Vertex[2].y;
            // double px = point.x, py = point.y;

            // double D = 2 * (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by));
            // if (D == 0) return false; // Degenerate triangle (points are collinear)

            // double asq = ax * ax + ay * ay;
            // double bsq = bx * bx + by * by;
            // double csq = cx * cx + cy * cy;
            // double psq = px * px + py * py;

            // double val = (asq * (by - cy) + bsq * (cy - ay) + csq * (ay - by));
            // double center_x = val / D;
            // double center_y = (asq * (cx - bx) + bsq * (ax - cx) + csq * (bx - ax)) / D;

            // double radiusSquared = (center_x - ax) * (center_x - ax) + (center_y - ay) * (center_y - ay);
            // double distSquared = (center_x - px) * (center_x - px) + (center_y - py) * (center_y - py);

            // double epsilon = 1e-6; // Using a small epsilon for floating point comparisons
            // return distSquared < radiusSquared - epsilon;
        }
        
        public bool HasSuperTrianglePoint(Point superP1, Point superP2, Point superP3)
        {
            return Vertex[0].Equals(superP1) || Vertex[0].Equals(superP2) || Vertex[0].Equals(superP3) ||
                Vertex[1].Equals(superP1) || Vertex[1].Equals(superP2) || Vertex[1].Equals(superP3) ||
                Vertex[2].Equals(superP1) || Vertex[2].Equals(superP2) || Vertex[2].Equals(superP3);
        }
    }
}
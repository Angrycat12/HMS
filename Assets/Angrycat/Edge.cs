namespace Angrycat
{
    public class Edge
    {
        public Point[] Vertex = new Point[2];

        public Point P1 => Vertex[0];
        public Point P2 => Vertex[1];

        public Edge(Point point1, Point point2)
        {
            Vertex[0] = point1;
            Vertex[1] = point2;
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge other)
            {
                return (P1.Equals(other.P1) && P2.Equals(other.P2)) || (P1.Equals(other.P2) && P2.Equals(other.P1));
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Симметричный хэш, не зависящий от порядка P1, P2
            return P1.GetHashCode() ^ P2.GetHashCode();
        }
    }
}
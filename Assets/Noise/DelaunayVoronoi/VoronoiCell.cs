using System.Collections.Generic;

namespace Noise
{
    public class VoronoiCell : Polygon
    {
        public Point CenterPoint;
        public List<VoronoiCell> Neighbours;

        public VoronoiCell(Point[] points, Point centerPoint) : base(points)
        {
            CenterPoint = centerPoint;
        }
    }
}
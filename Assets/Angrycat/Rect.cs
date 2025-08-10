namespace Angrycat
{
    /// <summary>
    /// Представляет выровненный по осям прямоугольник.
    /// </summary>
    public class Rect
    {
        /// <summary>
        /// Координата X левой стороны прямоугольника.
        /// </summary>
        public double x { get; private set; }

        /// <summary>
        /// Координата Y нижней стороны прямоугольника.
        /// </summary>
        public double y { get; private set; }

        /// <summary>
        /// Ширина прямоугольника.
        /// </summary>
        public double width { get; private set; }

        /// <summary>
        /// Высота прямоугольника.
        /// </summary>
        public double height { get; private set; }

        public double Left => x;
        public double Right => x + width;
        public double Bottom => y;
        public double Top => y + height;
        
        /// <summary>
        /// Инициализирует новый экземпляр Rect.
        /// </summary>
        /// <param name="x">Координата X левого нижнего угла.</param>
        /// <param name="y">Координата Y левого нижнего угла.</param>
        /// <param name="width">Ширина.</param>
        /// <param name="height">Высота.</param>
        public Rect(double X, double Y, double Width, double Height)
        {
            x = X;
            y = Y;
            width = Width;
            height = Height;
        }

        /// <summary>
        /// Проверяет, находится ли указанная точка внутри прямоугольника.
        /// </summary>
        /// <param name="point">Точка для проверки.</param>
        /// <returns>True, если точка внутри, иначе false.</returns>
        public bool Contains(Point point)
        {
            return point.x >= Left && point.x < Right && point.y >= Bottom && point.y < Top;
        }

        /// <summary>
        /// Проверяет, пересекается ли этот прямоугольник с другим.
        /// </summary>
        /// <param name="other">Другой прямоугольник для проверки.</param>
        /// <returns>True, если прямоугольники пересекаются, иначе false.</returns>
        public bool Overlaps(Rect other)
        {
            return Left < other.Right && Right > other.Left && Bottom < other.Top && Top > other.Bottom;
        }
    }
}
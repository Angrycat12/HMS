using System;
using System.Collections.Generic;
using System.Linq;

namespace Angrycat.Noise
{
    // Вспомогательный класс для обрезки многоугольников по прямоугольной области
    public static class Clipper
    {
        // Вспомогательная функция для векторного произведения (Z-компонента)
        // Возвращает положительное значение, если 'b' находится слева от вектора 'oa'
        // Возвращает отрицательное значение, если 'b' находится справа от вектора 'oa'
        // Возвращает ноль, если o, a, b коллинеарны
        private static double CrossProduct(Point o, Point a, Point b)
        {
            return (a.x - o.x) * (b.y - o.y) - (a.y - o.y) * (b.x - o.x);
        }

        // Проверяет, находится ли точка p "внутри" области отсечения относительно ребра cp1 -> cp2.
        // Предполагается, что внутренняя часть области отсечения находится СЛЕВА от направленного ребра (ориентация против часовой стрелки).
        // Добавлена небольшая погрешность (epsilon) для учета ошибок с плавающей точкой.
        private static bool IsInside(Point p, Point cp1, Point cp2)
        {
            double cross = CrossProduct(cp1, cp2, p);
            // Более надежное значение epsilon для сравнений.
            // Это позволяет точкам, которые из-за погрешности оказались чуть-чуть снаружи,
            // быть включенными в обрезанный многоугольник.
            const double robustEpsilon = 1e-5; // Возвращаем к разумному значению

            // Точка считается "внутри" или "на границе", если кросс-продукт >= -robustEpsilon.
            return cross >= -robustEpsilon; 
        }

        // Вычисляет точку пересечения отрезка s-e с отсекающей линией cp1-cp2.
        // Предполагается, что 's' - это точка СНАРУЖИ, а 'e' - точка ВНУТРИ.
        // Точка пересечения находится на отрезке от s до e.
        private static Point Intersect(Point s, Point e, Point cp1, Point cp2)
        {
            double cp_s = CrossProduct(cp1, cp2, s); // Знаковое расстояние от s до линии (cp1, cp2)
            double cp_e = CrossProduct(cp1, cp2, e); // Знаковое расстояние от e до линии (cp1, cp2)

            // Проверяем знаменатель на очень малое значение, чтобы избежать деления на ноль.
            // Если он слишком мал, это означает, что отрезки почти параллельны или коллинеарны.
            double denominator = cp_s - cp_e;
            if (Math.Abs(denominator) < 1e-9) // Использование 1e-9 для проверки на очень маленькое значение
            {
                // В этом вырожденном случае возвращаем 'e' (точку, которая находится внутри).
                // Это предотвратит NaN в координатах.
                return e; 
            }

            // Вычисляем параметр 't' для точки пересечения на отрезке s-e
            // Точка пересечения = s + t * (e - s)
            double t = cp_s / denominator; 
            
            // Клампируем t в диапазон [0, 1] для гарантии, что точка пересечения лежит на отрезке s-e.
            // Это особенно важно для предотвращения выхода за границы из-за ошибок с плавающей точкой.
            t = Math.Max(0.0, Math.Min(1.0, t));

            // Корректная параметрическая интерполяция: s + t * (e - s)
            return new Point(s.x + t * (e.x - s.x), s.y + t * (e.y - s.y));
        }

        /// <summary>
        /// Реализация алгоритма Сазерленда-Ходжмана для обрезки многоугольника одним отсекающим ребром.
        /// Входной многоугольник и отсекающее ребро должны быть в порядке против часовой стрелки (CCW).
        /// </summary>
        /// <param name="inputPolygon">Список вершин входного многоугольника (порядок CCW).</param>
        /// <param name="clipEdgeStart">Начальная точка отсекающего ребра.</param>
        /// <param name="clipEdgeEnd">Конечная точка отсекающего ребра (определяет направление). Внутренняя область находится слева от этого ребра.</param>
        /// <returns>Список вершин обрезанного многоугольника (порядок CCW).</returns>
        private static List<Point> SutherlandHodgmanClip(List<Point> inputPolygon, Point clipEdgeStart, Point clipEdgeEnd)
        {
            List<Point> outputList = new List<Point>();
            // Если входной многоугольник пуст, возвращаем пустой список
            if (inputPolygon == null || inputPolygon.Count == 0) return outputList;

            // Начинаем с последней вершины, чтобы сформировать первое ребро (s -> e)
            Point s = inputPolygon[inputPolygon.Count - 1]; 

            foreach (Point e in inputPolygon)
            {
                bool sIsInside = IsInside(s, clipEdgeStart, clipEdgeEnd);
                bool eIsInside = IsInside(e, clipEdgeStart, clipEdgeEnd);

                if (eIsInside) // Текущая вершина 'e' находится внутри границы отсечения
                {
                    if (!sIsInside) // Предыдущая вершина 's' была снаружи, пересекаем внутрь
                    {
                        outputList.Add(Intersect(s, e, clipEdgeStart, clipEdgeEnd));
                    }
                    outputList.Add(e); // Добавляем текущую вершину 'e'
                }
                else // Текущая вершина 'e' находится снаружи границы отсечения
                {
                    if (sIsInside) // Предыдущая вершина 's' была внутри, пересекаем наружу
                    {
                        outputList.Add(Intersect(s, e, clipEdgeStart, clipEdgeEnd));
                    }
                    // 'e' не добавляется, так как она снаружи
                }
                s = e; // Переходим к следующему ребру
            }
            return outputList;
        }

        /// <summary>
        /// Обрезает многоугольник по заданному прямоугольному ограничивающему блоку [minX,minY] до [maxX, maxY].
        /// Вершины входного многоугольника должны быть в порядке против часовой стрелки (CCW).
        /// </summary>
        /// <param name="polygonVertices">Список вершин многоугольника (порядок CCW).</param>
        /// <param name="minX">Минимальная X координата прямоугольника.</param>
        /// <param name="minY">Минимальная Y координата прямоугольника.</param>
        /// <param name="maxX">Максимальная X координата прямоугольника.</param>
        /// <param name="maxY">Максимальная Y координата прямоугольника.</param>
        /// <returns>Новый список вершин обрезанного многоугольника (порядок CCW), или пустой список, если ничего не осталось.</returns>
        public static List<Point> ClipPolygonToRectangle(List<Point> polygonVertices, double minX, double minY, double maxX, double maxY)
        {
            List<Point> clipped = new List<Point>(polygonVertices);
            
            // Ранний выход, если входной многоугольник вырожден
            if (clipped.Count < 3) return new List<Point>();

            // Определяем отсекающие ребра в порядке против часовой стрелки вокруг внутренней части прямоугольника.
            // Это гарантирует, что "внутренняя" часть области отсечения всегда находится СЛЕВА от направленного ребра.

            // 1. Отсечение по НИЖНЕМУ ребру: (minX, minY) -> (maxX, minY)
            // (Вектор направлен вправо, внутренняя область - выше/слева)
            clipped = SutherlandHodgmanClip(clipped, new Point(minX, minY), new Point(maxX, minY));
            // Проверка после каждого этапа, на случай если многоугольник стал вырожденным
            if (clipped.Count < 3) return new List<Point>(); 

            // 2. Отсечение по ПРАВОМУ ребру: (maxX, minY) -> (maxX, maxY)
            // (Вектор направлен вверх, внутренняя область - слева)
            clipped = SutherlandHodgmanClip(clipped, new Point(maxX, minY), new Point(maxX, maxY));
            if (clipped.Count < 3) return new List<Point>(); 

            // 3. Отсечение по ВЕРХНЕМУ ребру: (maxX, maxY) -> (minX, maxY)
            // (Вектор направлен влево, внутренняя область - ниже/слева)
            clipped = SutherlandHodgmanClip(clipped, new Point(maxX, maxY), new Point(minX, maxY));
            if (clipped.Count < 3) return new List<Point>(); 

            // 4. Отсечение по ЛЕВОМУ ребру: (minX, maxY) -> (minX, minY)
            // (Вектор направлен вниз, внутренняя область - справа/слева)
            clipped = SutherlandHodgmanClip(clipped, new Point(minX, maxY), new Point(minX, minY));
            
            // Финальная проверка после всех операций отсечения
            if (clipped.Count < 3) return new List<Point>();

            return clipped;
        }
    }
    

    public static class Voronoi
    {
        /// <summary>
        /// Генерирует диаграмму Вороного с релаксацией Ллойда.
        /// </summary>
        /// <param name="points">Начальный список точек-генераторов.</param>
        /// <param name="maxWidth">Максимальная ширина области генерации.</param>
        /// <param name="maxHeight">Максимальная высота области генерации.</param>
        /// <param name="numRelaxations">Количество итераций релаксации Ллойда.</param>
        /// <returns>Список ячеек Вороного (VoronoiCell).</returns>
        public static List<VoronoiCell> GenerateNoise(
            List<Point> points,
            double maxWidth,
            double maxHeight,
            int numRelaxations)
        {
            if (points == null || points.Count < 3)
            {
                throw new ArgumentException("Для генерации Вороного требуется как минимум 3 начальные точки.");
            }

            List<Point> currentPoints = new(points);

            // Начинаем с единицы что-б кол-во релоксаций совпадало с кол-во генераций вороного
            for (int i = 1; i < numRelaxations; i++)
            {
                List<DelaunayTriangle> delaunayTriangles = Delaunay.GenerateNoise(currentPoints);
                var voronoiCells = BuildVoronoiDiagram(delaunayTriangles, currentPoints, maxWidth, maxHeight, false); // замена true на false (а я то демал почему неправельно генерится диограмма, а это нейронка напортачила)

                // Собираем новые центроиды для следующей итерации
                currentPoints.Clear();
                foreach (var cell in voronoiCells)
                {
                    // Используем Centroid из Polygon, который вычисляется при создании VoronoiCell
                    // Это центроид обрезанной ячейки.
                    currentPoints.Add(cell.Centroid);
                }
            }

            // После всех релаксаций, строим окончательную диаграмму
            List<DelaunayTriangle> finalDelaunayTriangles = Delaunay.GenerateNoise(currentPoints);
            return BuildVoronoiDiagram(finalDelaunayTriangles, currentPoints, maxWidth, maxHeight, true); // Финальная обрезка
        }

        private static List<VoronoiCell> BuildVoronoiDiagram(
            List<DelaunayTriangle> delaunayTriangles,
            List<Point> originalPoints, // Эти точки - генераторы
            double maxWidth,
            double maxHeight,
            bool clipCells)
        {
            // Словарь для хранения вершин Вороного для каждой точки-генератора
            Dictionary<Point, List<Point>> cellVerticesRaw = new Dictionary<Point, List<Point>>();
            
            // Словарь для связи ребер Делоне с центрами описанных окружностей соседних треугольников
            Dictionary<Edge, Tuple<Point, Point>> edgeCircumcenters = new Dictionary<Edge, Tuple<Point, Point>>();

            foreach (var t in delaunayTriangles)
            {
                // Каждая вершина ячейки Вороного - это Circumcenter треугольника Делоне
                if (!cellVerticesRaw.ContainsKey(t.Vertex[0])) cellVerticesRaw[t.Vertex[0]] = new List<Point>();
                if (!cellVerticesRaw.ContainsKey(t.Vertex[1])) cellVerticesRaw[t.Vertex[1]] = new List<Point>();
                if (!cellVerticesRaw.ContainsKey(t.Vertex[2])) cellVerticesRaw[t.Vertex[2]] = new List<Point>();

                cellVerticesRaw[t.Vertex[0]].Add(t.Circumcenter);
                cellVerticesRaw[t.Vertex[1]].Add(t.Circumcenter);
                cellVerticesRaw[t.Vertex[2]].Add(t.Circumcenter);

                // Заполняем edgeCircumcenters
                Edge[] edges = { t.Edges[0], t.Edges[1], t.Edges[2] };
                foreach (Edge edge in edges)
                {
                    if (!edgeCircumcenters.ContainsKey(edge))
                    {
                        edgeCircumcenters[edge] = new Tuple<Point, Point>(t.Circumcenter, null);
                    }
                    else
                    {
                        var existing = edgeCircumcenters[edge];
                        // Если Circumcenter уже есть, значит это общее ребро для двух треугольников
                        edgeCircumcenters[edge] = new Tuple<Point, Point>(existing.Item1, t.Circumcenter);
                    }
                }
            }

            List<VoronoiCell> voronoiCells = new List<VoronoiCell>();

            // Вспомогательный словарь для быстрого поиска ячейки по её центральной точке (генератору)
            // Мы заполним его после создания всех ячеек, чтобы иметь полные объекты VoronoiCell
            Dictionary<Point, VoronoiCell> pointToCellMap = new Dictionary<Point, VoronoiCell>();


            // Строим ячейки Вороного
            foreach (var originalPoint in originalPoints) // Итерируем по исходным точкам-генераторам
            {
                if (!cellVerticesRaw.ContainsKey(originalPoint))
                {
                    // Это может случиться, если точка была отфильтрована (например, из-за супер-треугольника)
                    // или если она вырожденная. Пропускаем.
                    continue;
                }

                List<Point> vertices = cellVerticesRaw[originalPoint].Distinct().ToList(); // Убираем дубликаты

                // Важно: отсортировать вершины ячейки по углу вокруг центральной точки
                // Это необходимо, чтобы получить корректный многоугольник для обрезки
                if (vertices.Count > 0)
                {
                    // Центр для сортировки углов - это сама точка-генератор
                    Point sortCenter = originalPoint;

                    vertices.Sort((a, b) =>
                    {
                        double angleA = Math.Atan2(a.y - sortCenter.y, a.x - sortCenter.x);
                        double angleB = Math.Atan2(b.y - sortCenter.y, b.x - sortCenter.x);
                        return angleA.CompareTo(angleB);
                    });
                }
                
                List<Point> finalVertices = vertices;

                // Обрезка ячеек Вороного по maxWidth/maxHeight
                if (clipCells && finalVertices.Count >= 3) // Обрезаем только если есть полноценный полигон
                {
                    // Обрезаем полигон по границам
                    finalVertices = Clipper.ClipPolygonToRectangle(finalVertices, 0, 0, maxWidth, maxHeight);
                }

                if (finalVertices.Count >= 3) // Многоугольник должен иметь минимум 3 вершины после обрезки
                {
                    var cell = new VoronoiCell(finalVertices.ToArray(), originalPoint);
                    voronoiCells.Add(cell);
                    pointToCellMap[originalPoint] = cell; // Добавляем в маппинг
                }
                else
                {
                    // Ячейка стала вырожденной после обрезки (например, если ее полностью отсекло)
                    // Можем проигнорировать или добавить пустую/неполную ячейку
                }
            }

            // Добавление соседей (на основе общих ребер Делоне)
            foreach (var t in delaunayTriangles)
            {
                Edge[] delaunayEdges = { t.Edges[0], t.Edges[1], t.Edges[2] };
                foreach (Edge de in delaunayEdges)
                {
                    if (edgeCircumcenters.TryGetValue(de, out var tuple))
                    {
                        Point cc1 = tuple.Item1;
                        Point cc2 = tuple.Item2;

                        if (cc1 != null && cc2 != null) // Если ребро общее для двух треугольников
                        {
                            // Точки-генераторы, связанные с этим ребром (вершины ребра Делоне)
                            Point pA = de.P1;
                            Point pB = de.P2;

                            // Теперь нужно найти, какие ячейки Вороного соответствуют pA и pB
                            if (pointToCellMap.TryGetValue(pA, out VoronoiCell cellA) &&
                                pointToCellMap.TryGetValue(pB, out VoronoiCell cellB))
                            {
                                // Убедимся, что соседи не являются самими собой и ещё не добавлены
                                if (cellA != cellB)
                                {
                                    if (cellA.Neighbours == null) cellA.Neighbours = new List<VoronoiCell>();
                                    if (cellB.Neighbours == null) cellB.Neighbours = new List<VoronoiCell>();

                                    if (!cellA.Neighbours.Contains(cellB)) cellA.Neighbours.Add(cellB);
                                    if (!cellB.Neighbours.Contains(cellA)) cellB.Neighbours.Add(cellA);
                                }
                            }
                        }
                    }
                }
            }

            return voronoiCells;
        }
    }
}
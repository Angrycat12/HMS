using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class VoronoiPolygonGenerator : MonoBehaviour
{
    public int width = 256;
    public int height = 256;
    public int pointCount = 10;
    public GameObject parent;
    public GameObject polygonPrefab;

    private void Start()
    {
        GenerateVoronoiPolygons(width, height, pointCount);
    }

    private void GenerateVoronoiPolygons(int width, int height, int pointCount)
    {
        Vector2[] points = new Vector2[pointCount];
        Color[] colors = new Color[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            points[i] = new Vector2(Random.Range(0, width), Random.Range(0, height));
            colors[i] = new Color(Random.value, Random.value, Random.value);
        }

        Dictionary<int, List<Vector2>> polygons = new Dictionary<int, List<Vector2>>();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float minDistance = float.MaxValue;
                int closestPointIndex = 0;

                for (int i = 0; i < pointCount; i++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), points[i]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPointIndex = i;
                    }
                }

                if (!polygons.ContainsKey(closestPointIndex))
                {
                    polygons[closestPointIndex] = new List<Vector2>();
                }
                polygons[closestPointIndex].Add(new Vector2(x, y));
            }
        }

        foreach (var polygon in polygons)
        {
            Vector2[] orderedVertices = OrderVertices(polygon.Value);
            CreatePolygon(orderedVertices, colors[polygon.Key]);
        }
    }

    private List<float> NormalizeArray(List<float> array)
    {
        float maxvalue = array.Max();
        List<float> answer = new List<float>();
        foreach (float value in array) {
            answer.Add(value / maxvalue);
        }
        return answer;
    }

    private List<float> FindDistanse(List<Vector2> points, Vector2 center)
    {
        List<float> answer = new List<float>();
        foreach (Vector2 point in points) {
            answer.Add(Mathf.Sqrt(Mathf.Pow(point.x - center.x, 2) + Mathf.Pow(point.y - center.y, 2)));
        }
        return answer;
    }

    private Vector2[] OrderVertices(List<Vector2> vertices)
    {
        // https://habr.com/ru/articles/597449/
        Vector2 center = vertices.Aggregate(Vector2.zero, (acc, v) => acc + v) / vertices.Count;
        
        return vertices.OrderBy(v => Mathf.Atan2(v.y - center.y, v.x - center.x)).ToArray();
    }

    private void CreatePolygon(Vector2[] vertices, Color color)
    {
        GameObject polygon = Instantiate(polygonPrefab, Vector3.zero, Quaternion.identity);
        polygon.transform.SetParent(parent.transform);

        PolygonCollider2D collider = polygon.AddComponent<PolygonCollider2D>();
        collider.points = vertices;

        SpriteRenderer renderer = polygon.GetComponent<SpriteRenderer>();
        renderer.color = color;
    }
}


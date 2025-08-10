using System.Collections.Generic;
using System.Linq;
using Angrycat;
using Angrycat.Noise;
using R3;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class BiomeBinder : MonoBehaviour
{
    [SerializeField, ReadOnly]private float AvarageHeight;
    [SerializeField, ReadOnly]private float AvarageHumidity;
    [SerializeField, ReadOnly]private float AvarageTemperature;
    [SerializeField, ReadOnly]private float AvarageVegetation;
    [SerializeField] private Material Unknown;
    private readonly CompositeDisposable _disposables = new();

    private BiomeViewModel _viewModel;
    private int Width;
    private int Height;

    public void Bind(BiomeViewModel viewModel)
    {
        _viewModel = viewModel;
        Width = viewModel.Width.Value;
        Height = viewModel.Height.Value;

        _disposables.Add(viewModel.AvarageHeight.Subscribe(e => AvarageHeight = e));
        _disposables.Add(viewModel.AvarageHumidity.Subscribe(e => AvarageHumidity = e));
        _disposables.Add(viewModel.AvarageTemperature.Subscribe(e => AvarageTemperature = e));
        _disposables.Add(viewModel.AvarageVegetation.Subscribe(e => AvarageVegetation = e));

        Visualize();
    }

    private void Visualize()
    {
        var polygon = _viewModel.Biome.Origin.Polygon;
        polygon.Triangulate();
        int[] triangles = GetMeshTriangles(polygon);

        HashSet<Vector3> vertices = new();
        foreach (Edge edge in polygon.Edges)
        {
            vertices.Add(edge.P1.ToVector3());
            vertices.Add(edge.P2.ToVector3());
        }

        // Получаем MeshFilter, чтобы задать ему наш меш
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

        // Создаем новый меш
        Mesh mesh = new()
        {
            // Назначаем вершины и треугольники мешу
            vertices = PointsAlignmentV3(vertices, Width, Height),
            triangles = triangles,
        };

        // Обновляем меш
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Назначаем меш компоненту MeshFilter
        meshFilter.mesh = mesh;
        gameObject.GetComponent<MeshRenderer>().material = Unknown;
    }

    private static Vector3[] PointsAlignmentV3(HashSet<Vector3> vertices, int Width, int Height)
    {
        List<Vector3> newVertices = new();
        foreach(Vector3 vertex in vertices)
        {
            Vector3 newVertex = new Vector3(vertex.x - (Width/2), vertex.y - (Height/2), 0);
            newVertices.Add(newVertex);
        }
        return newVertices.ToArray();
    }

    private static int[] GetMeshTriangles(VoronoiCell polygon)
    {
        var triangles = polygon.Triangulate();
        var points = polygon.Vertex.ToList();

        // Создаем словарь для индексации вершин
        Dictionary<Vector2, int> vertexIndexMap = new Dictionary<Vector2, int>();
        for (int i = 0; i < points.Count; i++)
        {
            vertexIndexMap[points[i].ToVector2()] = i;
        }

        List<int> answer = new();
        foreach (var triangle in triangles)
        {
            answer.Add(vertexIndexMap[triangle.Vertex[0].ToVector2()]);
            answer.Add(vertexIndexMap[triangle.Vertex[1].ToVector2()]);
            answer.Add(vertexIndexMap[triangle.Vertex[2].ToVector2()]);
        }
        return answer.ToArray();
    }

    private void OnEnable()
    {
        if (_viewModel is not null)
        {
            _disposables.Add(_viewModel.AvarageHeight.Subscribe(e => AvarageHeight = e));
            _disposables.Add(_viewModel.AvarageHumidity.Subscribe(e => AvarageHumidity = e));
            _disposables.Add(_viewModel.AvarageTemperature.Subscribe(e => AvarageTemperature = e));
            _disposables.Add(_viewModel.AvarageVegetation.Subscribe(e => AvarageVegetation = e));
        }
    }

    private void OnDisable()
    {
        _disposables.Dispose();
    }
}
using UnityEngine;

public class MapTest : MonoBehaviour
{
    [SerializeField] private GameObject WorldGenerator;
    [SerializeField] public int Width;
    [SerializeField] public int Height;
    [SerializeField] public int Seed;

    void Start()
    {
        Seed = Random.Range(0, 10000);
        // WorldGenerator.GetComponent<WorldGenerator>().GenerateMap(new Param(Width, Height, Seed));
    }
}
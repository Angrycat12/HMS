using R3;

public class CmdCreatePerlinMap : ICommand
{
    public readonly ReactiveProperty<float[,]> Map;
    public readonly int Width;
    public readonly int Height;
    public readonly int Seed;
    public readonly int Scale;
    public readonly float Amplitude;
    public readonly float Frequency;
    public readonly float Period;
    public readonly int Octaves;

    public CmdCreatePerlinMap(ReactiveProperty<float[,]> map, int width, int height, int seed, int scale, float amplitude = 1f, float frequency = 1f, float period = 1f, int octaves = 4)
    {
        Map = map;
        Width = width;
        Height = height;
        Seed = seed;
        Scale = scale;
        Amplitude = amplitude;
        Frequency = frequency;
        Period = period;
        Octaves = octaves;
    }
}
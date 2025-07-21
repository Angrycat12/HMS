public class CmdCreateHeightMap : ICommand
{
    public readonly int WorldId;
    public readonly int Width;
    public readonly int Height;
    public readonly int Seed;
    public readonly int Scale;

    public CmdCreateHeightMap(int worldId, int width, int height, int seed, int scale)
    {
        WorldId = worldId;
        Width = width;
        Height = height;
        Seed = seed;
        Scale = scale;
    }
}
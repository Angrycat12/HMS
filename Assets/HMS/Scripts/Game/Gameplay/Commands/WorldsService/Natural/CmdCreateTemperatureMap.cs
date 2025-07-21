public class CmdCreateTemperatureMap: ICommand
{
    public readonly int WorldId;
    public readonly int Width;
    public readonly int Height;
    public readonly int Seed;

    public CmdCreateTemperatureMap(int worldId, int width, int height, int seed)
    {
        WorldId = worldId;
        Width = width;
        Height = height;
        Seed = seed;
    }
}
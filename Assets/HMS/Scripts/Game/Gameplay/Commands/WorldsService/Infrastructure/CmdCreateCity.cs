class CmdCreateCity : ICommand
{
    public readonly int WorldId;
    public readonly int Width;
    public readonly int Height;
    public readonly int Count;
    public readonly float[,] QualityOfCityLocations;

    public CmdCreateCity(int worldId, int width, int height, int count, float[,] qualityOfCityLocations)
    {
        WorldId = worldId;
        Width = width;
        Height = height;
        Count = count;
        QualityOfCityLocations = qualityOfCityLocations;
    }
}
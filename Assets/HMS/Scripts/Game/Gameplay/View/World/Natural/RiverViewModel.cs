using System;

public class RiverViewModel : IDisposable
{
    public readonly River River;

    public RiverViewModel(WorldService worldService, River river)
    {
        River = river;
    }

    public void Dispose()
    {

    }
}
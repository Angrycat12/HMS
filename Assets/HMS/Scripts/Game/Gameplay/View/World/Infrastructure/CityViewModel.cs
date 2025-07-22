using System;

public class CityViewModel : IDisposable
{
    public readonly City City;

    public CityViewModel(WorldService worldService, City city)
    {
        City = city;
    }

    public void Dispose()
    {

    }
}
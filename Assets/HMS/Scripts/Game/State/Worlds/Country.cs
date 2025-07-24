using System.Collections.Generic;
using R3;

public class Country
{
    public readonly CountryData Origin;
    public readonly ReactiveProperty<string> Name;
    public readonly List<Region> Regions;
    public readonly ReactiveProperty<int> Population;

    public Country(CountryData data, List<Region> regions)
    {
        Origin = data;

        Name = new(data.name);
        Name.Subscribe(e => data.name = e);

        Regions = new();
        foreach (Region region in regions)
        {
            foreach (int regionId in data.regionsId)
            {
                if (region.Origin.id == regionId)
                {
                    Regions.Add(region);
                }
            }
        }
    }
}
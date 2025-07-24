using System.Collections.Generic;

public class Region
{
    public readonly RegionData Origin;
    public readonly City Capital;
    public readonly List<Biome> Biomes;

    public Region(RegionData data, List<City> cities, List<Biome> biomes)
    {
        Origin = data;

        foreach (City city in cities)
        {
            if (city.Origin.id == data.cityId)
            {
                Capital = city;
                break;
            }
        }

        Biomes = new();
        foreach (Biome biome in biomes)
        {
            foreach (int biomeId in data.biomesId)
            {
                if (biome.Origin.Id == biomeId)
                {
                    Biomes.Add(biome);
                }
            }
        }
    }
}
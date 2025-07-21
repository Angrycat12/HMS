using System.Collections.Generic;
using ObservableCollections;
using R3;

public class World
{
    public readonly WorldData Origin;
    public readonly ReactiveProperty<string> Name;
    public readonly ReactiveProperty<int> Width;
    public readonly ReactiveProperty<int> Height;
    public readonly ReactiveProperty<int> Seed;
    public readonly ReactiveProperty<float> WaterLevel;
    public readonly ReactiveProperty<float[,]> HeightMap;
    public readonly ReactiveProperty<float[,]> HumidityMap;
    public readonly ReactiveProperty<float[,]> TemperatureMap;
    public readonly ReactiveProperty<float[,]> VegetationMap;
    public readonly ObservableList<River> Rivers;
    public readonly ObservableList<Biome> Biomes;
    public readonly ObservableList<Region> Regions;
    public readonly ObservableList<Country> Countries;

    public World(WorldData data)
    {
        Origin = data;

        Name = new ReactiveProperty<string>(data.name);
        Name.Subscribe(e => data.name = e);

        Width = new ReactiveProperty<int>(data.width);
        Width.Subscribe(e => data.width = e);

        Height = new ReactiveProperty<int>(data.height);
        Height.Subscribe(e => data.height = e);

        Seed = new ReactiveProperty<int>(data.seed);
        Seed.Subscribe(e => data.seed = e);

        WaterLevel = new ReactiveProperty<float>(data.waterLevel);
        WaterLevel.Subscribe(e => data.waterLevel = e);

        HeightMap = new ReactiveProperty<float[,]>(data.heightMap);
        HeightMap.Subscribe(e => data.heightMap = e);

        HumidityMap = new ReactiveProperty<float[,]>(data.humidityMap);
        HumidityMap.Subscribe(e => data.humidityMap = e);

        TemperatureMap = new ReactiveProperty<float[,]>(data.temperatureMap);
        TemperatureMap.Subscribe(e => data.temperatureMap = e);

        VegetationMap = new ReactiveProperty<float[,]>(data.vegetationMap);
        VegetationMap.Subscribe(e => data.vegetationMap = e);

        List<Biome> biomes = new();
        if (data.biomes is not null)
        {
            foreach (BiomeData b_data in data.biomes)
            {
                biomes.Add(new Biome(b_data, HeightMap, HumidityMap, TemperatureMap, VegetationMap));
            }
        }
        Biomes = new(biomes);
        Biomes.ObserveAdd().Subscribe(e => data.biomes.Add(e.Value.Origin));
        Biomes.ObserveRemove().Subscribe(e => data.biomes.Remove(e.Value.Origin));

        List<River> rivers = new();
        if (data.rivers is not null)
        {
            foreach (RiverData r_data in data.rivers)
            {
                rivers.Add(new River(r_data));
            }
        }
        Rivers = new(rivers);
        Rivers.ObserveAdd().Subscribe(e => data.rivers.Add(e.Value.Origin));
        Rivers.ObserveRemove().Subscribe(e => data.rivers.Remove(e.Value.Origin));

        // Regions = new(data.regions);
        // Regions.ObserveChanged().Subscribe(e => data.regions = Regions.ToList());

        // Countries = new(data.countries);
        // Countries.ObserveChanged().Subscribe(e => data.countries = Countries.ToList());
    }

    public void BiomeAdd(BiomeData data)
    {
        Biomes.Add(new Biome(data, HeightMap, HumidityMap, TemperatureMap, VegetationMap));
    }
}
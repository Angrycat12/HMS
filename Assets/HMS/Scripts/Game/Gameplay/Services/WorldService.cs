using System;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using Unity.Mathematics;
using UnityEngine;

public class WorldService : IDisposable
{
    public readonly World world;
    public readonly ReactiveProperty<string> Name;
    public readonly ReactiveProperty<int> Width;
    public readonly ReactiveProperty<int> Height;
    public readonly ReactiveProperty<int> Seed;
    public readonly ReactiveProperty<float> WaterLevel;
    public readonly ReactiveProperty<int> BiomeCount;
    public readonly ReactiveProperty<int> RegionCount;
    public readonly ReactiveProperty<int> CountryCount;
    private int lloydRelaxations = 3;
    private readonly ICommandProcessor _cmd;
    private readonly CompositeDisposable _disposables = new();

    public WorldService(int worldId, ObservableList<World> worlds, ICommandProcessor cmd)
    {
        world = worlds.FirstOrDefault(w => w.Origin.id == worldId);

        Name = world.Name;

        WaterLevel = world.WaterLevel;

        _disposables.Add(world.Width.Skip(1).Subscribe(e => CreateMap()));
        Width = world.Width;

        _disposables.Add(world.Height.Skip(1).Subscribe(e => CreateMap()));
        Height = world.Height;

        _disposables.Add(world.Seed.Skip(1).Subscribe(e => CreateMap()));
        Seed = world.Seed;

        BiomeCount = new(100);
        _disposables.Add(BiomeCount.Skip(1).Subscribe(e => CreateBiomes()));

        _cmd = cmd;
    }

    public bool CreateMap()
    {
        return CreateHeightMap(1) && CreateHumidityMap(1) && CreateTemperatureMap() &&
            CreateVegetationMap(1) && CreateBiomes();
        // _cmd.Process(new CmdCreateRegion()) && _cmd.Process(new CmdCreateCountry());
    }

    public bool IsWorldGenerated()
    {
        return false;
    }

    #region GetViewModel
    public NoiseMapViewModel GetHeightViewModel()
    {
        return new NoiseMapViewModel(world.HeightMap);
    }

    public NoiseMapViewModel GetHumidityViewModel()
    {
        return new NoiseMapViewModel(world.HumidityMap);
    }

    public NoiseMapViewModel GetTemperatureViewModel()
    {
        return new NoiseMapViewModel(world.TemperatureMap);
    }

    public NoiseMapViewModel GetVegetationViewModel()
    {
        return new NoiseMapViewModel(world.VegetationMap);
    }

    public List<RiverViewModel> GetRiverViewModels()
    {
        List<RiverViewModel> answer = new();
        foreach (River river in world.Rivers)
        {
            answer.Add(new(this, river));
        }
        return answer;
    }

    public List<BiomeViewModel> GetBiomeViewModels()
    {
        List<BiomeViewModel> answer = new();
        foreach (Biome biome in world.Biomes)
        {
            answer.Add(new(this, biome));
        }
        return answer;
    }

    // public List<CityViewModel> GetCityViewModels()
    // {
    //     List<CityViewModel> answer = new();
    //     foreach (City city in world.Cities)
    //     {
    //         answer.Add(new(this, city));
    //     }
    //     return answer;
    // }
    #endregion

    #region Create
    public bool CreateHeightMap(int scale)
    {
        Debug.Log("Started create Height Map");
        return _cmd.Process(new CmdCreateHeightMap(world.Origin.id, Width.Value, Height.Value, Seed.Value, scale));
    }

    public bool CreateHumidityMap(int scale)
    {
        Debug.Log("Started create Humidity Map");
        return _cmd.Process(new CmdCreateHumidityMap(world.Origin.id, Width.Value, Height.Value, Seed.Value, scale));
    }

    public bool CreateTemperatureMap()
    {
        Debug.Log("Started create Temperature Map");
        return _cmd.Process(new CmdCreateTemperatureMap(world.Origin.id, Width.Value, Height.Value, Seed.Value));
    }

    public bool CreateVegetationMap(int scale)
    {
        Debug.Log("Started create Vegetation Map");
        return _cmd.Process(new CmdCreateVegetationMap(world.Origin.id, Width.Value, Height.Value, Seed.Value, scale));
    }

    public bool CreateRiver()
    {
        Debug.Log("Started create River Map");
        List<Vector2Int> sourceRiver = new() { new(UnityEngine.Random.Range(0, Width.Value), UnityEngine.Random.Range(0, Height.Value))};
        return _cmd.Process(new CmdCreateRiver(world.Origin.id, sourceRiver, world.HeightMap.Value, WaterLevel.Value));
    }

    public bool CreateBiomes()
    {
        Debug.Log("Started create Biome Map");
        return _cmd.Process(new CmdCreateBiomes(world.Origin.id, Width.Value, Height.Value, BiomeCount.Value, lloydRelaxations,
        world.HeightMap.Value, world.HumidityMap.Value, world.TemperatureMap.Value, world.VegetationMap.Value));
    }

    //  public bool CreateCities()
    //  {
    //      return _cmd.Process(new CmdCreateCity());
    //  }

    //  public bool CreateRoads()
    //  {
    //      return _cmd.Process(new CmdCreateRoad());
    //  }

    //  public bool CreateRegions()
    //  {
    //      return _cmd.Process(new CmdCreateRegions(world.Origin.id));
    //  }

    //  public bool CreateCountres()
    //  {
    //      return _cmd.Process(new CmdCreateCountres(world.Origin.id));
    //  }
    #endregion

    public BiomeType GetBiomeType(int biomeId, float height, float humidity, float temperature, float vegetation)
    {
        _cmd.Process(new CmdGetBiomeType());
        return new BiomeType();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
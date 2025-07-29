using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;

public class WorldService : IDisposable
{
    public readonly World world;
    public readonly ReactiveProperty<string> Name;
    public readonly ReactiveProperty<int> Width;
    public readonly ReactiveProperty<int> Height;
    public readonly ReactiveProperty<int> Seed;
    public readonly ReactiveProperty<float> WaterLevel;
    public readonly ReactiveProperty<int> CityCount;
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

        _disposables.Add(world.Width.Skip(1).Subscribe(async e => await CreateMap()));
        Width = world.Width;

        _disposables.Add(world.Height.Skip(1).Subscribe(async e => await CreateMap()));
        Height = world.Height;

        _disposables.Add(world.Seed.Skip(1).Subscribe(async e => await CreateMap()));
        Seed = world.Seed;

        BiomeCount = new(100);
        _disposables.Add(BiomeCount.Skip(1).Subscribe(async e => await CreateBiomes()));

        CityCount = new(2);

        _cmd = cmd;
    }

    public async Task<bool> CreateMap(CancellationToken cancellationToken = default)
    {
        bool answer = false;
        answer = await CreateHeightMap(1, 1, 1, 1, 4, cancellationToken);
        answer &= await CreateHumidityMap(1, 1, 1, 1, 4, cancellationToken);
        answer &= await CreateTemperatureMap(cancellationToken);
        answer &= await CreateVegetationMap(1, 1, 1, 1, 4, cancellationToken); 
        answer &= await CreateBiomes(cancellationToken);
        return answer;
        // _cmd.Process(new CmdCreateRegion()) && _cmd.Process(new CmdCreateCountry());
    }

    public bool IsWorldGenerated()
    {
        return false;
    }

    #region GetViewModel
    public NoiseMapViewModel GetHeightViewModel(CancellationToken cancellationToken = default)
    {
        ReactiveProperty<float> Amplitude = new(1);
        ReactiveProperty<float> Frequency = new(1);
        ReactiveProperty<float> Period = new(1);
        ReactiveProperty<int> Octaves = new(4);

        _disposables.Add(Amplitude.Merge(Frequency)
                                  .Merge(Period)
                                  .Subscribe(async e => await CreateHeightMap(1, Amplitude.Value, Frequency.Value, Period.Value, Octaves.Value, cancellationToken)));
        _disposables.Add(Octaves.Subscribe(async e => await CreateHeightMap(1, Amplitude.Value, Frequency.Value, Period.Value, Octaves.Value, cancellationToken)));

        return new NoiseMapViewModel(world.HeightMap, Amplitude, Frequency, Period, Octaves);
    }

    public NoiseMapViewModel GetHumidityViewModel(CancellationToken cancellationToken = default)
    {
        ReactiveProperty<float> Amplitude = new(1);
        ReactiveProperty<float> Frequency = new(1);
        ReactiveProperty<float> Period = new(1);
        ReactiveProperty<int> Octaves = new(4);

        _disposables.Add(Amplitude.Merge(Frequency)
                                  .Merge(Period)
                                  .Subscribe(async e => await CreateHumidityMap(1, Amplitude.Value, Frequency.Value, Period.Value, Octaves.Value, cancellationToken)));
        _disposables.Add(Octaves.Subscribe(async e => await CreateHumidityMap(1, Amplitude.Value, Frequency.Value, Period.Value, Octaves.Value, cancellationToken)));

        return new NoiseMapViewModel(world.HumidityMap, Amplitude, Frequency, Period, Octaves);
    }

    public NoiseMapViewModel GetTemperatureViewModel()
    {
        return new NoiseMapViewModel(world.TemperatureMap);
    }

    public NoiseMapViewModel GetVegetationViewModel(CancellationToken cancellationToken = default)
    {
        ReactiveProperty<float> Amplitude = new(1);
        ReactiveProperty<float> Frequency = new(1);
        ReactiveProperty<float> Period = new(1);
        ReactiveProperty<int> Octaves = new(4);

        _disposables.Add(Amplitude.Merge(Frequency)
                                  .Merge(Period)
                                  .Subscribe(async e => await CreateVegetationMap(1, Amplitude.Value, Frequency.Value, Period.Value, Octaves.Value, cancellationToken)));
        _disposables.Add(Octaves.Subscribe(async e => await CreateVegetationMap(1, Amplitude.Value, Frequency.Value, Period.Value, Octaves.Value, cancellationToken)));

        return new NoiseMapViewModel(world.VegetationMap, Amplitude, Frequency, Period, Octaves);
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

    public List<CityViewModel> GetCityViewModels()
    {
        List<CityViewModel> answer = new();
        foreach (City city in world.Cities)
        {
            answer.Add(new(this, city));
        }
        return answer;
    }
    #endregion

    #region Create
    public async Task<bool> CreateHeightMap(int scale, float amplitude = 1f, float frequency = 1f, float period = 1f, int octaves = 4, CancellationToken cancellationToken = default)
    {
        Debug.Log("Started create Height Map");
        return await _cmd.Process(new CmdCreatePerlinMap(world.HeightMap, Width.Value, Height.Value, Seed.Value, scale, amplitude, frequency, period, octaves), cancellationToken);
    }

    public async Task<bool> CreateHumidityMap(int scale, float amplitude = 1f, float frequency = 1f, float period = 1f, int octaves = 4, CancellationToken cancellationToken = default)
    {
        Debug.Log("Started create Humidity Map");
        return await _cmd.Process(new CmdCreatePerlinMap(world.HumidityMap, Width.Value, Height.Value, Seed.Value, scale, amplitude, frequency, period, octaves), cancellationToken);
    }

    public async Task<bool> CreateTemperatureMap(CancellationToken cancellationToken = default)
    {
        Debug.Log("Started create Temperature Map");
        return await _cmd.Process(new CmdCreateTemperatureMap(world.Origin.id, Width.Value, Height.Value, Seed.Value), cancellationToken);
    }

    public async Task<bool> CreateVegetationMap(int scale, float amplitude = 1f, float frequency = 1f, float period = 1f, int octaves = 4, CancellationToken cancellationToken = default)
    {
        Debug.Log("Started create Vegetation Map");
        return await _cmd.Process(new CmdCreatePerlinMap(world.VegetationMap, Width.Value, Height.Value, Seed.Value, scale, amplitude, frequency, period, octaves), cancellationToken);
    }

    public async Task<bool> CreateRiver(CancellationToken cancellationToken = default)
    {
        Debug.Log("Started create River Map");
        List<Vector2Int> sourceRiver = new() { new(UnityEngine.Random.Range(0, Width.Value), UnityEngine.Random.Range(0, Height.Value))};
        return await _cmd.Process(new CmdCreateRiver(world.Origin.id, sourceRiver, world.HeightMap.Value, WaterLevel.Value), cancellationToken);
    }

    public async Task<bool> CreateBiomes(CancellationToken cancellationToken = default)
    {
        Debug.Log("Started create Biome Map");
        return await _cmd.Process(new CmdCreateBiomes(world.Origin.id, Width.Value, Height.Value, BiomeCount.Value, lloydRelaxations,
        world.HeightMap.Value, world.HumidityMap.Value, world.TemperatureMap.Value, world.VegetationMap.Value), cancellationToken);
    }

    public async Task<bool> CreateCities(CancellationToken cancellationToken = default)
    {
        return _cmd.Process(new CmdCreateCity(world.Origin.id, Width.Value, Height.Value, CityCount.Value, world.QualityOfCityLocations.Value));
    }

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
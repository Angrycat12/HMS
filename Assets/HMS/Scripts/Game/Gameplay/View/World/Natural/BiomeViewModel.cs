using System;
using System.Linq;
using ObservableCollections;
using R3;

public class BiomeViewModel : IDisposable
{
    public readonly Biome Biome;

    public readonly ReactiveProperty<int> Width;
    public readonly ReactiveProperty<int> Height;

    public readonly ReactiveProperty<float> AvarageHeight;
    public readonly ReactiveProperty<float> AvarageHumidity;
    public readonly ReactiveProperty<float> AvarageTemperature;
    public readonly ReactiveProperty<float> AvarageVegetation;

    public BiomeType BiomeType;

    private int _id;
    private WorldService _worldsService;
    private readonly CompositeDisposable _disposables = new();

    public BiomeViewModel(WorldService worldService, Biome biome, BiomeType biomeType = null)
    {
        Biome = biome;

        Width = worldService.Width;
        Height = worldService.Height;

        _worldsService = worldService;

        AvarageHeight = new(biome.Height.Average());
        _disposables.Add(biome.Height.ObserveChanged().Subscribe(e => AvarageHeight.Value = biome.Height.Average()));

        // AvarageHumidity = new(biome.Humidity.Average());
        // _disposables.Add(biome.Humidity.ObserveChanged().Subscribe( e => AvarageHumidity.Value = biome.Humidity.Average()));

        // AvarageTemperature = new(biome.Temperature.Average());
        // _disposables.Add(biome.Temperature.ObserveChanged().Subscribe( e => AvarageTemperature.Value = biome.Temperature.Average()));

        // AvarageVegetation = new(biome.Vegetation.Average());
        // _disposables.Add(biome.Vegetation.ObserveChanged().Subscribe( e => AvarageVegetation.Value = biome.Vegetation.Average()));

        _disposables.Add(AvarageHeight
        // .Merge(AvarageHumidity)
        //              .Merge(AvarageTemperature)
        //              .Merge(AvarageVegetation)
                        .Subscribe(e => GetBiomeType()));

        if (biomeType is not null)
        {
            BiomeType = biomeType;
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    private void GetBiomeType()
    {
        // BiomeType = _worldsService.GetBiomeType(_id, AvarageHeight.Value, AvarageHumidity.Value, AvarageTemperature.Value, AvarageVegetation.Value);
    }
}
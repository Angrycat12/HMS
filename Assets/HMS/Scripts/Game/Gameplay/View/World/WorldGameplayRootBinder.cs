using System.Collections.Generic;
using ObservableCollections;
using R3;
using UnityEngine;

public class WorldGameplayRootBinder : MonoBehaviour
{
    [Header("Parametrs")]
    public string Name;
    [Range(1, 10000)] public int Width;
    [Range(1, 10000)] public int Height;
    [Range(0, 999999)] public int Seed;
    [Range(0, 1)] public float WaterLevel;
    [Range(1, 1000)] public int BiomeCount;

    [Header("Natural")]
    [SerializeField] private NoiseMapBinder _prefabHeightMap;
    [SerializeField] private NoiseMapBinder _prefabHumidityMap;
    [SerializeField] private NoiseMapBinder _prefabTemperatureMap;
    [SerializeField] private NoiseMapBinder _prefabVegetationMap;
    [SerializeField] private BiomeBinder _prefabBiome;
    // [SerializeField] private RiverBinder _prefabRiver;

    // [Header("Infrastructure")]
    // [SerializeField] private CityBinder _prefabCity;
    // [SerializeField] private RoadBinder _prefabRoad;

    [Header("Political")]
    [SerializeField] private RegionBinder _prefabRegion;
    [SerializeField] private CountryBinder _prefabCountry;

    private readonly CompositeDisposable _disposables = new();

    private ReactiveProperty<string> _name;
    private ReactiveProperty<int> _width;
    private ReactiveProperty<int> _height;
    private ReactiveProperty<int> _seed;
    private ReactiveProperty<float> _waterLevel;
    private ReactiveProperty<int> _biomeCount;

    private WorldGameplayRootViewModel _viewModel;

    public void Bind(WorldGameplayRootViewModel viewModel)
    {
        _viewModel = viewModel;

        Name = viewModel.Name.Value;
        Width = viewModel.Width.Value;
        Height = viewModel.Height.Value;
        Seed = viewModel.Seed.Value;
        WaterLevel = viewModel.WaterLevel.Value;
        BiomeCount = viewModel.BiomeCount.Value;

        _name = viewModel.Name;
        _width = viewModel.Width;
        _height = viewModel.Height;
        _seed = viewModel.Seed;
        _waterLevel = viewModel.WaterLevel;
        _biomeCount = viewModel.BiomeCount;

        CreateNoiseMap(viewModel.HeightViewModel, _prefabHeightMap);
        // CreateNoiseMap(viewModel.HumidityViewModel, _prefabHumidityMap);
        // CreateNoiseMap(viewModel.TemperatureViewModel, _prefabTemperatureMap);
        // CreateNoiseMap(viewModel.VegetationViewModel, _prefabVegetationMap);

        viewModel.BiomeViewModels.ForEach(m => CreateBiome(m));
        _disposables.Add(viewModel.BiomeViewModels.ObserveAdd().Subscribe(e =>
        {
            CreateBiome(e.Value);
        }
        ));
        _disposables.Add(viewModel.BiomeViewModels.ObserveRemove().Subscribe(e =>
        {
            DeleteBiome(e.Value);
        }
        ));
        // _disposables.Add(viewModel.RegionViewModels.ObserveAdd().Subscribe(e =>
        // {
        //     CreateRegion(e.Value);
        // }
        // ));
        // _disposables.Add(viewModel.RegionViewModels.ObserveRemove().Subscribe(e =>
        // {
        //     DeleteRegion(e.Value);
        // }
        // ));
        // _disposables.Add(viewModel.CountryViewModels.ObserveAdd().Subscribe(e =>
        // {
        //     CreateCountry(e.Value);
        // }
        // ));
        //  _disposables.Add(viewModel.CountryViewModels.ObserveRemove().Subscribe(e =>
        // {
        //     DeleteCountry(e.Value);
        // }
        // ));
    }

    private void CreateNoiseMap(NoiseMapViewModel noiseViewModel, NoiseMapBinder noiseMapBinder)
    {
        Instantiate(noiseMapBinder).Bind(noiseViewModel);
    }

    private void CreateBiome(BiomeViewModel biomeViewModel)
    {
        Instantiate(_prefabBiome).Bind(biomeViewModel);
    }

    private void DeleteBiome(BiomeViewModel biomeViewModel)
    {
        // Destroy();
    }

    private void CreateRegion(RegionViewModel regionViewModel)
    {
        Instantiate(_prefabRegion).Bind(regionViewModel);
    }

    private void DeleteRegion(RegionViewModel biomeViewModel)
    {
        // Instantiate(_prefabBiome).Bind(biomeViewModel);
    }

    private void CreateCountry(CountryViewModel countryViewModel)
    {
        Instantiate(_prefabCountry).Bind(countryViewModel);
    }

    private void DeleteCountry(CountryViewModel biomeViewModel)
    {
        // Instantiate(_prefabBiome).Bind(biomeViewModel);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}

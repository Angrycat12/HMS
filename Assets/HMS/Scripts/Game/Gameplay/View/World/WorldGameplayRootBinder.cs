using System.Collections.Generic;
using System.Linq;
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
    [Range(1, 1000)] public int CityCount;
    [Range(1, 1000)] public int RegionCount;
    [Range(1, 1000)] public int CountryCount;

    [Header("Natural")]
    [SerializeField] private NoiseMapBinder _prefabHeightMap;
    [SerializeField] private NoiseMapBinder _prefabHumidityMap;
    [SerializeField] private NoiseMapBinder _prefabTemperatureMap;
    [SerializeField] private NoiseMapBinder _prefabVegetationMap;
    [SerializeField] private BiomeBinder _prefabBiome;
    private List<GameObject> Biomes = new();
    [SerializeField] private RiverBinder _prefabRiver;
    private List<GameObject> Rivers = new();

    [Header("Infrastructure")]
    [SerializeField] private CityBinder _prefabCity;
    private List<GameObject> Cities = new();
    [SerializeField] private RoadBinder _prefabRoad;
    private List<GameObject> Roads = new();

    [Header("Political")]
    [SerializeField] private RegionBinder _prefabRegion;
    private List<GameObject> Regions = new();
    [SerializeField] private CountryBinder _prefabCountry;
    private List<GameObject> Country = new();

    private readonly CompositeDisposable _disposables = new();

    private ReactiveProperty<string> _name;
    private ReactiveProperty<int> _width;
    private ReactiveProperty<int> _height;
    private ReactiveProperty<int> _seed;
    private ReactiveProperty<float> _waterLevel;
    private ReactiveProperty<int> _biomeCount;
    private ReactiveProperty<int> _cityCount;
    private ReactiveProperty<int> _regionCount;
    private ReactiveProperty<int> _countryCount;

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
        CreateNoiseMap(viewModel.HumidityViewModel, _prefabHumidityMap);
        CreateNoiseMap(viewModel.TemperatureViewModel, _prefabTemperatureMap);
        CreateNoiseMap(viewModel.VegetationViewModel, _prefabVegetationMap);

        viewModel.RiverViewModels.ForEach(m => CreateRiver(m));
        _disposables.Add(viewModel.RiverViewModels.ObserveAdd().Subscribe(e =>
        {
            CreateRiver(e.Value);
        }
        ));
        _disposables.Add(viewModel.RiverViewModels.ObserveRemove().Subscribe(e =>
        {
            DeleteRiver(e.Value);
        }
        ));

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

        viewModel.CityViewModels.ForEach(m => CreateCity(m));
        _disposables.Add(viewModel.CityViewModels.ObserveAdd().Subscribe(e =>
        {
            CreateCity(e.Value);
        }
        ));
        _disposables.Add(viewModel.CityViewModels.ObserveRemove().Subscribe(e =>
        {
            DeleteCity(e.Value);
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

    #region Create
        private void CreateNoiseMap(NoiseMapViewModel noiseViewModel, NoiseMapBinder noiseMapBinder)
        {
            Instantiate(noiseMapBinder).Bind(noiseViewModel);
        }

        private void CreateRiver(RiverViewModel riverViewModel)
        {
            var river = Instantiate(_prefabRiver, gameObject.transform);
            river.Bind(riverViewModel);
            Rivers.Add(river.gameObject);
        }

        private void CreateBiome(BiomeViewModel biomeViewModel)
        {
            var biome = Instantiate(_prefabBiome, gameObject.transform);
            biome.Bind(biomeViewModel);
            Biomes.Add(biome.gameObject);
        }

        private void CreateCity(CityViewModel cityViewModel)
        {
            var city = Instantiate(_prefabCity, cityViewModel.City.Origin.position, Quaternion.identity, gameObject.transform);
            city.Bind(cityViewModel);
            Cities.Add(city.gameObject);
        }

        private void CreateRoad(RoadViewModel roadViewModel)
        {
            Instantiate(_prefabRoad, gameObject.transform).Bind(roadViewModel);
        }
    
        private void CreateRegion(RegionViewModel regionViewModel)
        {
            Instantiate(_prefabRegion, gameObject.transform).Bind(regionViewModel);
        }
    
        private void CreateCountry(CountryViewModel countryViewModel)
        {
            Instantiate(_prefabCountry, gameObject.transform).Bind(countryViewModel);
        }
    #endregion

    #region Delete
        private void DeleteRiver(RiverViewModel riverViewModel)
        {
            Destroy(Rivers.ElementAt(riverViewModel.River.Origin.id));
        }
    
        private void DeleteBiome(BiomeViewModel biomeViewModel)
        {
            Destroy(Biomes.ElementAt(biomeViewModel.Biome.Origin.Id));
        }
    
        private void DeleteCity(CityViewModel cityViewModel)
        {
            Destroy(Cities.ElementAt(cityViewModel.City.Origin.id));
        }

        private void DeleteRoad(RoadViewModel roadViewModel)
        {
            // Destroy(Roads.ElementAt(roadViewModel.Road.Origin.id));
        }
    
        private void DeleteRegion(RegionViewModel biomeViewModel)
        {
        
        }
    
        private void DeleteCountry(CountryViewModel biomeViewModel)
        {
            
        }
    #endregion

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}

using System.Threading;
using System.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEngine;


public class WorldGameplayRootViewModel : UIRootViewModel
{
    // propertise
    public readonly ReactiveProperty<string> Name;
    public readonly ReactiveProperty<int> Width;
    public readonly ReactiveProperty<int> Height;
    public readonly ReactiveProperty<int> Seed;
    public readonly ReactiveProperty<float> WaterLevel;
    public readonly ReactiveProperty<int> BiomeCount;
    // Natural
    public NoiseMapViewModel HeightViewModel;
    public NoiseMapViewModel HumidityViewModel;
    public NoiseMapViewModel TemperatureViewModel;
    public NoiseMapViewModel VegetationViewModel;
    public ObservableList<RiverViewModel> RiverViewModels;
    public ObservableList<BiomeViewModel> BiomeViewModels;
    // Infrastructure
    public ObservableList<CityViewModel> CityViewModels;
    public ObservableList<RoadViewModel> RoadViewModels;
    // Political
    public ObservableList<RegionViewModel> RegionViewModels;
    public ObservableList<CountryViewModel> CountryViewModels;

    private readonly WorldService _worldService;

    public WorldGameplayRootViewModel(WorldService worldService)
    {
        _worldService = worldService;
        Name = worldService.Name;
        Width = worldService.Width;
        Height = worldService.Height;
        Seed = worldService.Seed;
        WaterLevel = worldService.WaterLevel;
        BiomeCount = worldService.BiomeCount;
    }

    public async Task Start(CancellationToken cancellationToken = default)
    {
        if (!_worldService.IsWorldGenerated())
        {
            Debug.Log("start");
            await _worldService.CreateMap(cancellationToken);
        }
        
        HeightViewModel = _worldService.GetHeightViewModel();
        HumidityViewModel = _worldService.GetHumidityViewModel();
        TemperatureViewModel = _worldService.GetTemperatureViewModel();
        VegetationViewModel = _worldService.GetVegetationViewModel();
        RiverViewModels = new(_worldService.GetRiverViewModels());
        BiomeViewModels = new(_worldService.GetBiomeViewModels());

        CityViewModels = new(_worldService.GetCityViewModels());
        // RoadViewModels = new(_worldService.GetRoadViewModels());

        // RegionViewModels = new(_worldService.GetRegionViewModels());
        // CountryViewModel = mew(_worldService.GetCountryViewModels());
        
    }
}
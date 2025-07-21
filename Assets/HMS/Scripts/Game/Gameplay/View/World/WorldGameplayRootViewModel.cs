using ObservableCollections;
using R3;


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
    public ObservableList<BiomeViewModel> BiomeViewModels;
    // Political
    public ObservableList<RegionViewModel> RegionViewModels;
    public ObservableList<CountryViewModel> CountryViewModels;

    public WorldGameplayRootViewModel(WorldService worldService)
    {
        Name = worldService.Name;
        Width = worldService.Width;
        Height = worldService.Height;
        Seed = worldService.Seed;
        WaterLevel = worldService.WaterLevel;
        BiomeCount = worldService.BiomeCount;

        if (!worldService.IsWorldGenerated())
        {
            worldService.CreateMap();
        }

        HeightViewModel = worldService.GetHeightViewModel();
        HumidityViewModel = worldService.GetHumidityViewModel();
        TemperatureViewModel = worldService.GetTemperatureViewModel();
        VegetationViewModel = worldService.GetVegetationViewModel();
        BiomeViewModels = new(worldService.GetBiomeViewModels());
        
        // RegionViewModels = new(worldService.GetRegionViewModels());
        // CountryViewModel = mew(worldService.GetCountryViewModels());
    }
}
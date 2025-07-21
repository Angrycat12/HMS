using ObservableCollections;
using R3;

public class Biome
{
    public readonly BiomeData Origin;
    public readonly ObservableList<float> Height = new();
    public readonly ObservableList<float> Temperature = new();
    public readonly ObservableList<float> Humidity = new();
    public readonly ObservableList<float> Vegetation = new();

    public Biome(BiomeData biomeData, ReactiveProperty<float[,]> height, ReactiveProperty<float[,]> humidity, ReactiveProperty<float[,]> temperature, ReactiveProperty<float[,]> vegetation)
    {
        Origin = biomeData;

        height.Subscribe(e =>
        {
            for (int i = 0; i < biomeData.Points.Count; i++)
            {
                Height.Insert(i, e[biomeData.Points[i][0], biomeData.Points[i][1]]);
            }
        });
        // humidity.Subscribe(e =>
        // {
        //     for (int i = 0; i < biomeData.Points.Count; i++)
        //     {
        //         Humidity.Insert(i, e[biomeData.Points[i][0], biomeData.Points[i][1]]);
        //     }
        // });
        // temperature.Subscribe(e =>
        // {
        //     for (int i = 0; i < biomeData.Points.Count; i++)
        //     {
        //         Temperature.Insert(i, e[biomeData.Points[i][0], biomeData.Points[i][1]]);
        //     }
        // });
        // vegetation.Subscribe(e =>
        // {
        //     for (int i = 0; i < biomeData.Points.Count; i++)
        //     {
        //         Vegetation.Insert(i, e[biomeData.Points[i][0], biomeData.Points[i][1]]);
        //     }
        // });
    }
}
using R3;

public class NoiseMapViewModel
{
    public readonly ReactiveProperty<float[,]> Map;

    public NoiseMapViewModel(ReactiveProperty<float[,]> data)
    {
        Map = data;
    }
}
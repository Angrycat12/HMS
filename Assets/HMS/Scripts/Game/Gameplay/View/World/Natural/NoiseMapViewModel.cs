using R3;

public class NoiseMapViewModel
{
    public readonly ReactiveProperty<float[,]> Map;
    public readonly ReactiveProperty<float> Amplitude;
    public readonly ReactiveProperty<float> Frequency;
    public readonly ReactiveProperty<float> Period;
    public readonly ReactiveProperty<int> Octaves;

    public NoiseMapViewModel(ReactiveProperty<float[,]> data)
    {
        Map = data;
    }

    public NoiseMapViewModel(ReactiveProperty<float[,]> data, ReactiveProperty<float> amplitude, ReactiveProperty<float> frequency, ReactiveProperty<float> period, ReactiveProperty<int> octaves)
    {
        Map = data;
        Amplitude = amplitude;
        Frequency = frequency;
        Period = period;
        Octaves = octaves;
    }
}
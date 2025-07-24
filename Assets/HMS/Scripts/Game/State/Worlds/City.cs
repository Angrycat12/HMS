using R3;

public class City
{
    public readonly CityData Origin;
    public readonly ReactiveProperty<string> Name;
    public readonly ReactiveProperty<int> Population;

    public City(CityData data)
    {
        Origin = data;

        Name = new(data.name);
        Name.Subscribe(e => data.name = e);

        Population = new(data.population);
        Population.Subscribe(e => data.population = e);
    }
}
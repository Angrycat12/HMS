using System.Threading.Tasks;

public interface ISettingsProvider
{
    GameSettings GameSettings { get; }
    ApplicationSettings ApplicationSettings { get; }

    Task<GameSettings> LoadGameSettings();
}

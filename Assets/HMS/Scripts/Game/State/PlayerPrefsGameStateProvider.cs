using System.Collections.Generic;
using R3;
using UnityEngine;

public class PlayerPrefsGameStateProvider : IGameStateProvider
{
    public GameStateProxy GameState { get; private set; }
    public GameSettingsStateProxy SettingsState { get; private set; }

    private const string GAME_STATE_KEY = nameof(GAME_STATE_KEY);
    private const string GAME_SETTINGS_STATE_KEY = nameof(GAME_SETTINGS_STATE_KEY);

    private GameState _gameStateOrigin;
    private GameSettingsState _gameSettingsStateOrigin;

    public Observable<GameStateProxy> LoadGameState()
    {
        if (!PlayerPrefs.HasKey(GAME_STATE_KEY))
        {
            GameState = CreateGameStateFromSettings();
            Debug.Log("Game State created from settings: " + JsonUtility.ToJson(_gameStateOrigin, true));

            SaveGameState();    // Сохраним дефолтное состояние
        }
        else
        {
            // Загружаем
            var json = PlayerPrefs.GetString(GAME_STATE_KEY);
            _gameStateOrigin = JsonUtility.FromJson<GameState>(json);
            GameState = new GameStateProxy(_gameStateOrigin);
            
            Debug.Log("Game State loaded: " + json);
        }

        return Observable.Return(GameState);
    }
    
    public Observable<GameSettingsStateProxy> LoadSettingsState()
    {
        if (!PlayerPrefs.HasKey(GAME_SETTINGS_STATE_KEY))
        {
            SettingsState = CreateGameSettingsStateFromSettings();
            
            SaveSettingsState();    // Сохраним дефолтное состояние
        }
        else
        {
            // Загружаем
            var json = PlayerPrefs.GetString(GAME_SETTINGS_STATE_KEY);
            _gameSettingsStateOrigin = JsonUtility.FromJson<GameSettingsState>(json);
            SettingsState = new GameSettingsStateProxy(_gameSettingsStateOrigin);
        }

        return Observable.Return(SettingsState);
    }

    public Observable<bool> SaveGameState()
    {
        var json = JsonUtility.ToJson(_gameStateOrigin, true);
        PlayerPrefs.SetString(GAME_STATE_KEY, json);

        return Observable.Return(true);
    }
    
    public Observable<bool> SaveSettingsState()
    {
        var json = JsonUtility.ToJson(_gameSettingsStateOrigin, true);
        PlayerPrefs.SetString(GAME_SETTINGS_STATE_KEY, json);

        return Observable.Return(true);
    }

    public Observable<bool> ResetGameState()
    {
        GameState = CreateGameStateFromSettings();
        SaveGameState();
        
        return Observable.Return(true);
    }
    
    public Observable<GameSettingsStateProxy> ResetSettingsState()
    {
        SettingsState = CreateGameSettingsStateFromSettings();
        SaveSettingsState();
        
        return Observable.Return(SettingsState);
    }
    
    private GameStateProxy CreateGameStateFromSettings()
    {
        // Состояние по умолчанию из настроек
        _gameStateOrigin = new GameState
        {
            Worlds = new List<WorldData>()
            {
                new() { id = 0, name = "", seed = 000000, width = 1500, height = 1000, waterLevel = 0.02f }
            },
            Resources = new List<ResourceData>()
            {
                new() { Amount = 0, ResourceType = ResourceType.SoftCurrency },
                new() { Amount = 0, ResourceType = ResourceType.HardCurrency },
                new() { Amount = 0, ResourceType = ResourceType.Wood }
            }
        };
            
        return new GameStateProxy(_gameStateOrigin);
    }
    
    private GameSettingsStateProxy CreateGameSettingsStateFromSettings()
    {
        // Состояние по умолчанию из настроек, мы делаем фейк
        _gameSettingsStateOrigin = new GameSettingsState()
        {
            MusicVolume = 8,
            SFXVolume = 8
        };
            
        return new GameSettingsStateProxy(_gameSettingsStateOrigin);
    }
}
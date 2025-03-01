using System;
using System.Collections.Generic;

[Serializable]
public class PlayerPrefsWrapper
{
    private PlayerPrefsHandler _playerPrefsHandler = new();

    private const string KEY_PLAYER_ID = "PlayerID";
    private const string KEY_PLAYER_NAME = "PlayerName";
    private const string KEY_BINDING = "Binding";
    private const string KEY_UNLOCKED_COLLECTIBLES = "Collectibles";
    private const string KEY_TUTORIAL_SETTINGS = "TutorialSettings";
    private const string KEY_AUDIO_SETTINGS = "AudioSettings";

    public void SavePlayerId(string playerId)
    {
        _playerPrefsHandler.SaveData(KEY_PLAYER_ID, playerId);
    }

    public string LoadPlayerId()
    {
        return _playerPrefsHandler.LoadData<string>(KEY_PLAYER_ID);
    }

    public void DeletePlayerId()
    {
        _playerPrefsHandler.DeleteData(KEY_PLAYER_ID);
    }

    public void SavePlayerName(string playerName)
    {
        _playerPrefsHandler.SaveData(KEY_PLAYER_NAME, playerName);
    }

    public string LoadPlayerName()
    {
        return _playerPrefsHandler.LoadData<string>(KEY_PLAYER_NAME);
    }

    public void DeletePlayerName()
    {
        _playerPrefsHandler.DeleteData(KEY_PLAYER_NAME);
    }

    public void SaveActionBinding(string currentBindings)
    {
        _playerPrefsHandler.SaveData(KEY_BINDING, currentBindings);
    }

    public string LoadActionBinding()
    {
        return _playerPrefsHandler.LoadData<string>(KEY_BINDING);
    }

    public void SaveCollectibles(UnlockedCollectibleData collectibles)
    {
        _playerPrefsHandler.SaveData(KEY_UNLOCKED_COLLECTIBLES, collectibles);
    }

    public UnlockedCollectibleData LoadCollectibles()
    {
        return _playerPrefsHandler.LoadData<UnlockedCollectibleData>(KEY_UNLOCKED_COLLECTIBLES);
    }

    public void DeleteCollectibles()
    {
        _playerPrefsHandler.DeleteData(KEY_UNLOCKED_COLLECTIBLES);
    }

    public void SaveTutorialSettings(TutorialSettings tutorialSettings)
    {
        _playerPrefsHandler.SaveData(KEY_TUTORIAL_SETTINGS, tutorialSettings);
    }

    public TutorialSettings LoadCompletedTutorials()
    {
        return _playerPrefsHandler.LoadData<TutorialSettings>(KEY_TUTORIAL_SETTINGS);
    }

    public void DeleteCompletedTutorials()
    {
        _playerPrefsHandler.DeleteData(KEY_TUTORIAL_SETTINGS);
    }

    public void SaveAudioSettings(Dictionary<SoundGroup, float> audioSettings)
    {
        _playerPrefsHandler.SaveData(KEY_AUDIO_SETTINGS, audioSettings);
    }

    public Dictionary<SoundGroup, float> LoadAudioSettings()
    {
        return _playerPrefsHandler.LoadData<Dictionary<SoundGroup, float>>(KEY_AUDIO_SETTINGS);
    }

    public void DeleteAudioSettings()
    {
        _playerPrefsHandler.DeleteData(KEY_AUDIO_SETTINGS);
    }
}

using System;

[Serializable]
public class PlayerPrefsWrapper
{
    private PlayerPrefsHandler _playerPrefsHandler = new();

    private const string KEY_PLAYER_ID = "PlayerID";
    private const string KEY_PLAYER_NAME = "PlayerName";
    private const string KEY_BINDING = "Binding";
    private const string KEY_UNLOCKED_COLLECTIBLES = "Collectibles";

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
}

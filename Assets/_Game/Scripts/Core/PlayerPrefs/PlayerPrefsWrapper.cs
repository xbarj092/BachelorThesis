using System;

[Serializable]
public class PlayerPrefsWrapper
{
    private PlayerPrefsHandler _playerPrefsHandler = new();

    private const string KEY_PLAYER_ID = "PlayerID";

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
}

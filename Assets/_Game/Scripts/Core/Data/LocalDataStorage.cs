using System;
using UnityEngine;

public class LocalDataStorage : MonoSingleton<LocalDataStorage>
{
    [field: SerializeField] public PlayerData PlayerData;
    [field: SerializeField] public GameData GameData;

    private void Awake()
    {
        InitPlayerData();
    }

    public void InitPlayerData()
    {
        PlayerData.PlayerStats = new(0, 2, 2, 5, 10);
    }
}

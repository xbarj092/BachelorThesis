using System;
using System.Collections.Generic;
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
        PlayerData.PlayerStats = new(0, 15, 15, 5, 10);
        PlayerData.InventoryData = new(new List<Item> { null, null, null, null, null, null });
    }
}

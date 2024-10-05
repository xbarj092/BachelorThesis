using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private PlayerStats _playerStats;
    public PlayerStats PlayerStats
    {
        get => _playerStats;
        set
        {
            _playerStats = value;
            DataEvents.OnPlayerStatsChangedInvoke(_playerStats);
        }
    }

    [SerializeField] private InventoryData _inventoryData;
    public InventoryData InventoryData
    {
        get => _inventoryData;
        set
        {
            _inventoryData = value;
            DataEvents.OnInventoryDataChangedInvoke(_inventoryData);
        }
    }
}

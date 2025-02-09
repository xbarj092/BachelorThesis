using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private TransformData _playerTransform;
    public TransformData PlayerTransform
    {
        get => _playerTransform;
        set
        {
            _playerTransform = value;
        }
    }

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

    [SerializeField] private SavedInventoryData _savedInventoryData;
    public SavedInventoryData SavedInventoryData
    {
        get => _savedInventoryData;
        set
        {
            _savedInventoryData = value;
            DataEvents.OnSavedInventoryDataChangedInvoke(_savedInventoryData);
        }
    }

    [SerializeField] private UnlockedCollectibleData _unlockedCollectibleData = new();
    public UnlockedCollectibleData UnlockedCollectibleData
    {
        get => _unlockedCollectibleData;
        set
        {
            _unlockedCollectibleData = value;
            DataEvents.OnUnlockedCollectibleDataChangedInvoke(_unlockedCollectibleData);
        }
    }
}

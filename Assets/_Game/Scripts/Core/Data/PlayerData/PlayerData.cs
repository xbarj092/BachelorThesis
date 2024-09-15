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
}

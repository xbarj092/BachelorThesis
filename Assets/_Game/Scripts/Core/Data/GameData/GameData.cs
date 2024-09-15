using System;
using UnityEngine;

[Serializable]
public class GameData
{
    [SerializeField] private MapLayout _mapLayout;
    public MapLayout MapLayout
    {
        get => _mapLayout;
        set
        {
            _mapLayout = value;
            DataEvents.OnMapLayoutChangedInvoke(_mapLayout);
        }
    }
}

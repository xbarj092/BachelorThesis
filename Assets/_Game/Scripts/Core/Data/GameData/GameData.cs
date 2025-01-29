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

    [SerializeField] private KittenData _kittenData;
    public KittenData KittenData
    {
        get => _kittenData;
        set
        {
            _kittenData = value;
            DataEvents.OnKittenDataChangedInvoke(_kittenData);
        }
    }

    [SerializeField] private FoodData _foodData;
    public FoodData FoodData
    {
        get => _foodData;
        set
        {
            _foodData = value;
            DataEvents.OnFoodDataChangedInvoke(_foodData);
        }
    }

    [SerializeField] private ItemData _itemData;
    public ItemData ItemData
    {
        get => _itemData;
        set
        {
            _itemData = value;
            DataEvents.OnItemDataChangedInvoke(_itemData);
        }
    }
}

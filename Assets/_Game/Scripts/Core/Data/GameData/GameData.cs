using System;
using UnityEngine;

[Serializable]
public class GameData
{
    public System.Random Random;
    public GameSave CurrentSave;

    [SerializeField] private GameSeeds _gameSeeds;
    public GameSeeds GameSeeds
    {
        get => _gameSeeds;
        set
        {
            _gameSeeds = value;
            Random = new(_gameSeeds.MapGenerationSeed);
            DataEvents.OnGameSeedsChangedInvoke(_gameSeeds);
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

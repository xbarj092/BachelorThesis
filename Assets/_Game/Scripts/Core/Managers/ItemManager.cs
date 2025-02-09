using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoSingleton<ItemManager>
{
    [SerializeField] private SerializedDictionary<ItemType, UseableItem> _useableItems = new();
    [SerializeField] private SerializedDictionary<ConsumableType, ConsumableItem> _consumableItems = new();

    public List<UseableItem> SpawnedItems = new();
    public List<UseableItem> SavedItems = new();

    public List<ConsumableItem> SpawnedConsumables = new();
    public List<ConsumableItem> SavedConsumables = new();

    private List<int> _spawnedItemUIDs = new();

    public void SaveItems()
    {
        LocalDataStorage.Instance.GameData.ItemData.SavedItems.Clear();
        SavedItems.Clear();
        LocalDataStorage.Instance.GameData.ItemData.SavedConsumables.Clear();
        SavedConsumables.Clear();

        foreach (UseableItem item in SpawnedItems)
        {
            item.SaveItem();
        }

        foreach (ConsumableItem item in SpawnedConsumables)
        {
            item.SaveItem();
        }
    }

    public void LoadItems()
    {
        SpawnedItems.Clear();
        SpawnedConsumables.Clear();

        foreach (SavedUseableItem item in LocalDataStorage.Instance.GameData.ItemData.SavedItems)
        {
            UseableItem spawnedItem = SpawnItem((ItemType)item.ItemType);
            if (spawnedItem != null)
            {
                spawnedItem.LoadItem(item);
                SpawnedItems.Add(spawnedItem);
            }
        }

        foreach (SavedConsumableItem item in LocalDataStorage.Instance.GameData.ItemData.SavedConsumables)
        {
            ConsumableItem spawnedItem = SpawnConsumable((ConsumableType)item.ItemType);
            if (spawnedItem != null)
            {
                spawnedItem.LoadItem(item);
                SpawnedConsumables.Add(spawnedItem);
            }
        }
    }

    public UseableItem SpawnItem(ItemType itemType)
    {
        return Instantiate(_useableItems[itemType]);
    }

    public ConsumableItem SpawnConsumable(ConsumableType consumableType)
    {
        return Instantiate(_consumableItems[consumableType]);
    }

    public void SpawnItem(ItemType itemType, Vector2 spawnPosition, Quaternion spawnRotation, Transform parent)
    {
        UseableItem item = Instantiate(_useableItems[itemType], spawnPosition, spawnRotation, parent);
        if (item != null)
        {
            item.UID = SetItemUId();
            SpawnedItems.Add(item);
        }
    }

    public void SpawnConsumable(ConsumableType consumableType, Vector2 spawnPosition, Quaternion spawnRotation, Transform parent)
    {
        ConsumableItem item = Instantiate(_consumableItems[consumableType], spawnPosition, spawnRotation, parent);
        if (item != null)
        {
            item.UID = SetItemUId();
            SpawnedConsumables.Add(item);
        }
    }

    private int SetItemUId()
    {
        int uid = 0;
        do
        {
            uid = Random.Range(0, 100000);
        }
        while (_spawnedItemUIDs.Contains(uid));
        return uid;
    }

    public UseableItem GetItemWithUID(int uid)
    {
        return SpawnedItems.FirstOrDefault(item => item.UID == uid);
    }

    public ConsumableItem GetConsumableWithUID(int uid)
    {
        return SpawnedConsumables.FirstOrDefault(item => item.UID == uid);
    }

    public void ResetManager()
    {
        SpawnedItems.Clear();
        SpawnedConsumables.Clear();
        SavedItems.Clear();
        SavedConsumables.Clear();
        _spawnedItemUIDs.Clear();
    }
}

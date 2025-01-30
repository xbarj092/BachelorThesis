using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoSingleton<ItemManager>
{
    [SerializeField] private SerializedDictionary<ItemType, Item> _items = new();

    public List<Item> SpawnedItems = new();
    public List<Item> SavedItems = new();

    private List<int> _spawnedItemUIDs = new();

    public void SaveItems()
    {
        LocalDataStorage.Instance.GameData.ItemData.SavedItems.Clear();
        SavedItems.Clear();

        foreach (Item item in SpawnedItems)
        {
            item.SaveItem();
        }
    }

    public void LoadItems()
    {
        SpawnedItems.Clear();

        foreach (SavedItem item in LocalDataStorage.Instance.GameData.ItemData.SavedItems)
        {
            Item spawnedItem = SpawnItem((ItemType)item.ItemType);
            if (spawnedItem != null)
            {
                spawnedItem.LoadItem(item);
                SpawnedItems.Add(spawnedItem);
            }
        }
    }

    public Item SpawnItem(ItemType itemType)
    {
        return Instantiate(_items[itemType]);
    }

    public void SpawnItem(ItemType itemType, Vector2 spawnPosition, Quaternion spawnRotation, Transform parent)
    {
        Item item = Instantiate(_items[itemType], spawnPosition, spawnRotation, parent);
        if (item != null)
        {
            item.UID = SetItemUId();
            SpawnedItems.Add(item);
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

    public Item GetItemWithUID(int uid)
    {
        return SpawnedItems.FirstOrDefault(item => item.UID == uid);
    }

    public void ResetManager()
    {
        SpawnedItems.Clear();
        SavedItems.Clear();
        _spawnedItemUIDs.Clear();
    }
}

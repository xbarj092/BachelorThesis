using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDataStorage : MonoSingleton<LocalDataStorage>
{
    public PlayerData PlayerData;
    public GameData GameData;
    public PlayerPrefsWrapper PlayerPrefs;

    private DataSaver _dataSaver = new();
    private DataLoader _dataLoader = new();

    private void Awake()
    {
        InitPlayerData(false);
        UnlockedCollectibleData data = PlayerPrefs.LoadCollectibles();
        if (data != null)
        {
            PlayerData.UnlockedCollectibleData = data;
        }
    }

    public void InitPlayerData(bool loaded)
    {
        if (!loaded)
        {
            int spriteIndex = PlayerData.PlayerStats.SpriteIndex;
            PlayerData.PlayerStats = new(spriteIndex, 0, 150, 150, false);
            PlayerData.InventoryData = new(new List<UseableItem> { null, null, null, null, null, null });
        }
        else
        {
            PlayerData.InventoryData = new(new List<UseableItem> { null, null, null, null, null, null });

            InventoryData inventoryData = PlayerData.InventoryData;
            foreach (SavedUseableItem item in PlayerData.SavedInventoryData.SavedItems)
            {
                UseableItem inventoryItem = ItemManager.Instance.GetItemWithUID(item.UID);
                if (inventoryItem != null)
                {
                    for (int i = 0; i < inventoryData.ItemsInInventory.Count; i++)
                    {
                        if (inventoryData.ItemsInInventory[i] == null)
                        {
                            inventoryData.ItemsInInventory[i] = inventoryItem;
                            break;
                        }
                    }
                }
            }

            inventoryData.CurrentHighlightIndex = PlayerData.SavedInventoryData.CurrentHighlightIndex;
            PlayerData.InventoryData = inventoryData;
        }
    }

    public IEnumerator SaveData(byte[] image, Action onSuccess)
    {
        DataEvents.OnDataSavedInvoke();
        yield return new WaitForSecondsRealtime(0.5f);

        GameSave gameSave = new(image, PlayerData.PlayerTransform, PlayerData.PlayerStats, PlayerData.SavedInventoryData, 
            GameData.GameSeeds, GameData.KittenData, GameData.FoodData, GameData.ItemData);

        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss");
        string saveFileName = $"Save_{timestamp}.gg";
        string savePath = GlobalConstants.SavedDataPaths.BASE_PATH + "/" + saveFileName;

        _dataSaver.SaveData(gameSave, savePath);
        onSuccess?.Invoke();
    }

    public GameSave GetSaveFileFromPath(string path)
    {
        return _dataLoader.LoadData<GameSave>(GlobalConstants.SavedDataPaths.BASE_PATH + "/" + path);
    }

    public IEnumerator LoadData(GameSave gameSave)
    {
        PlayerPrefs.SavePlayerName(gameSave.Name);

        PlayerData.PlayerTransform = gameSave.PlayerTransform;
        PlayerData.PlayerStats = gameSave.PlayerStats;
        PlayerData.SavedInventoryData = gameSave.SavedInventoryData;

        GameData.GameSeeds = gameSave.GameSeeds;
        GameData.KittenData = gameSave.KittenData;
        GameData.FoodData = gameSave.FoodData;
        GameData.ItemData = gameSave.ItemData;

        yield return new WaitForSeconds(0.5f);
        DataEvents.OnDataLoadedInvoke();
    }
}

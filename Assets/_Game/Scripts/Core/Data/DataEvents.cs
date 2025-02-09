using System;

public static class DataEvents
{
    public static event Action<PlayerStats> OnPlayerStatsChanged;
    public static void OnPlayerStatsChangedInvoke(PlayerStats playerStats)
    {
        OnPlayerStatsChanged?.Invoke(playerStats);
    }

    public static event Action<InventoryData> OnInventoryDataChanged;
    public static void OnInventoryDataChangedInvoke(InventoryData inventoryData)
    {
        OnInventoryDataChanged?.Invoke(inventoryData);
    }

    public static event Action<SavedInventoryData> OnSavedInventoryDataChanged;
    public static void OnSavedInventoryDataChangedInvoke(SavedInventoryData savedInventoryData)
    {
        OnSavedInventoryDataChanged?.Invoke(savedInventoryData);
    }

    public static event Action<UnlockedCollectibleData> OnUnlockedCollectibleDataChanged;
    public static void OnUnlockedCollectibleDataChangedInvoke(UnlockedCollectibleData unlockedCollectibleData)
    {
        OnUnlockedCollectibleDataChanged?.Invoke(unlockedCollectibleData);
    }

    public static event Action<GameSeeds> OnGameSeedsChanged;
    public static void OnGameSeedsChangedInvoke(GameSeeds gameSeeds)
    {
        OnGameSeedsChanged?.Invoke(gameSeeds);
    }

    public static event Action<KittenData> OnKittenDataChanged;
    public static void OnKittenDataChangedInvoke(KittenData kittenData)
    {
        OnKittenDataChanged?.Invoke(kittenData);
    }

    public static event Action<FoodData> OnFoodDataChanged;
    public static void OnFoodDataChangedInvoke(FoodData foodData)
    {
        OnFoodDataChanged?.Invoke(foodData);
    }

    public static event Action<ItemData> OnItemDataChanged;
    public static void OnItemDataChangedInvoke(ItemData itemData)
    {
        OnItemDataChanged?.Invoke(itemData);
    }

    public static event Action OnDataSaved;
    public static void OnDataSavedInvoke()
    {
        OnDataSaved?.Invoke();
    }

    public static Action OnDataLoaded;
    public static void OnDataLoadedInvoke()
    {
        OnDataLoaded?.Invoke();
    }
}

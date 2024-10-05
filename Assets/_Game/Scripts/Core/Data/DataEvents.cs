using System;

public static class DataEvents
{
    public static event Action<CurrencyData> OnCurrencyDataChanged;
    public static void OnCurrencyDataChangedInvoke(CurrencyData currencyData)
    {
        OnCurrencyDataChanged?.Invoke(currencyData);
    }

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

    public static event Action<MapLayout> OnMapLayoutCHanged;
    public static void OnMapLayoutChangedInvoke(MapLayout mapLayout)
    {
        OnMapLayoutCHanged?.Invoke(mapLayout);
    }
}

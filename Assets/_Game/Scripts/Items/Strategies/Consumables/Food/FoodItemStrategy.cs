using UnityEngine;

public class FoodItemStrategy : ConsumableItemStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        int replenishAmount = ((FoodItemSO)item.Stats).ReplenishAmount;
        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        int secondsAdded;
        if (stats.CurrentTimeLeft + replenishAmount >= stats.MaxTimeLeft)
        {
            secondsAdded = Mathf.RoundToInt(stats.MaxTimeLeft - stats.CurrentTimeLeft);
            stats.CurrentTimeLeft = stats.MaxTimeLeft;
        }
        else
        {
            secondsAdded = replenishAmount;
            stats.CurrentTimeLeft += replenishAmount;
        }

        LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
        UGSAnalyticsManager.Instance.RecordFoodPickedUp(stats.TimeAlive, secondsAdded);
        item.gameObject.SetActive(false);
    }
}

public class FoodItemStrategy : ConsumableItemStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        int replenishAmount = ((FoodItemSO)item.Stats).ReplenishAmount;
        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        stats.CurrentTimeToEatFood += replenishAmount;

        while (stats.CurrentTimeToEatFood >= stats.TimeToEatFood)
        {
            if (stats.CurrentFood < stats.MaxFood)
            {
                stats.CurrentFood++;
                stats.CurrentTimeToEatFood -= stats.TimeToEatFood;
            }
            else
            {
                stats.CurrentTimeToEatFood = stats.TimeToEatFood;
                break;
            }
        }

        GameEvents.OnFoodStateChangedInvoke(replenishAmount);
        LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
        UGSAnalyticsManager.Instance.RecordFoodPickedUp(stats.TimeAlive, replenishAmount, item.Stats.ConsumableType.ToString());
        base.PickUp(item);
    }
}

public abstract class ConsumableItemStrategy : StrategyBase<ConsumableItem, ConsumableItemSO, SavedConsumableItem>, IConsumableStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        base.PickUp(item);
        LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.AddUseable(item.Stats);
    }
}

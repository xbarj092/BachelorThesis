public abstract class ConsumableItemStrategy : StrategyBase<ConsumableItem, ConsumableItemSO, SavedConsumableItem>, IConsumableStrategy
{
    public abstract override void PickUp(ConsumableItem item);
}

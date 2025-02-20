public abstract class ConsumableItemStrategy : StrategyBase<ConsumableItem, ConsumableItemSO, SavedConsumableItem>, IConsumableStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        base.PickUp(item);
        if (!TutorialManager.Instance.IsTutorialCompleted(item.Stats.ConsumableType))
        {
            TutorialManager.Instance.InstantiateTutorial(item.Stats.ConsumableType);
        }
        LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.AddUseable(item.Stats);
    }
}

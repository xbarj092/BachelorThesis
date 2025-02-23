public abstract class ConsumableItemStrategy : StrategyBase<ConsumableItem, ConsumableItemSO, SavedConsumableItem>, IConsumableStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        if (!TutorialManager.Instance.IsTutorialCompleted(item.Stats.ConsumableType) && !TutorialManager.Instance.IsTutorialPlaying())
        {
            TutorialManager.Instance.InstantiateTutorial(item.Stats.ConsumableType);
        }

        AudioManager.Instance.Play(SoundType.FoodPickedUp);

        LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.AddUseable(item.Stats);
        base.PickUp(item);
    }
}

public abstract class ConsumableItem : Item<ConsumableItemSO, SavedConsumableItem>
{
    private ConsumableItemStrategy _strategy;

    private void Awake()
    {
        _strategy = _strategyFactory.CreateStrategy(Stats.ConsumableType);
        _spriteRenderer.sprite = Stats.Sprite;
    }

    public override abstract void SaveItem();
}

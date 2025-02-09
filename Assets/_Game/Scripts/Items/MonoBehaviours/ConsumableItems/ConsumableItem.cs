public abstract class ConsumableItem : Item<ConsumableItemSO, SavedConsumableItem>
{
    private ConsumableItemStrategy _strategy;

    private void Awake()
    {
        _strategy = _strategyFactory.CreateStrategy(Stats.ConsumableType);
        _spriteRenderer.sprite = Stats.Sprite;
    }

    public override void PickUp()
    {
        _strategy.PickUp(this);
    }

    public override void SaveItem()
    {
        SavedConsumableItem savedItem = new(new(transform), (int)Stats.ConsumableType, UID, gameObject.activeInHierarchy);
        LocalDataStorage.Instance.GameData.ItemData.SavedConsumables.Add(savedItem);
    }
}

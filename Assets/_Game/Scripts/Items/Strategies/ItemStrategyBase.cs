public abstract class ItemStrategyBase : IItemStrategy
{
    public abstract void Use(Item item);
    public virtual void PickUp(Item item)
    {
        item.gameObject.SetActive(false);
    }
}

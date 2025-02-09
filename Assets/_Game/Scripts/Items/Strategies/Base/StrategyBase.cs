public abstract class StrategyBase<T, U, V> : IItemStrategy<T>
    where T : Item<U, V>
    where U : ItemBaseSO
    where V : SavedItem
{
    public virtual void PickUp(T item)
    {
        item.gameObject.SetActive(false);
    }
}

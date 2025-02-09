public interface IItemStrategy
{
    bool CanUse(UseableItem item);
    void Use(UseableItem item);
    void PickUp(UseableItem item);
}

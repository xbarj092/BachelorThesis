public interface IItemStrategy
{
    bool CanUse(Item item);
    void Use(Item item);
    void PickUp(Item item);
}

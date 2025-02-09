public interface IUseableStrategy
{
    bool CanUse(UseableItem item);
    void Use(UseableItem item);
}

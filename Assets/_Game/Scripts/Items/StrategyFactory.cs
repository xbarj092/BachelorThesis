public class StrategyFactory
{
    public ItemStrategyBase CreateStrategy(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Laser => new LaserItemStrategy(),
            ItemType.Mouse => new MouseItemStrategy(),
            ItemType.CastrationKit => new CastrationKitItemStrategy(),
            ItemType.Towel => new TowelItemStrategy(),
            ItemType.Clothespin => new ClothespinItemStrategy(),
            ItemType.CardboardBox => new CardboardBoxItemStrategy(),
            _ => null,
        };
    }
}

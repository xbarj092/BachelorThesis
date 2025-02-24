public class StrategyFactory
{
    public UseableItemStrategy CreateStrategy(ItemType itemType)
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

    public ConsumableItemStrategy CreateStrategy(ConsumableType consumableType)
    {
        return consumableType switch
        {
            ConsumableType.Fish => new FishItemStrategy(),
            ConsumableType.Steak => new SteakItemStrategy(),
            ConsumableType.GoldenApple => new GoldenAppleItemStrategy(),
            ConsumableType.InvisibilityPotion => new PotionItemStrategy(),
            _ => null,
        };
    }
}

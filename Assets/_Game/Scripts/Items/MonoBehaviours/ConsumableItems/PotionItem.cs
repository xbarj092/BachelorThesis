public class PotionItem : ConsumableItem
{
    public override void LoadItem(SavedConsumableItem item)
    {
        if ((ConsumableType)item.ItemType == Stats.ConsumableType)
        {
            item.ApplyToItem(this);
        }
    }
}

public class CastrationKit : UseableItem
{
    public override void SaveItem()
    {
        SavedUseableItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy);
        LocalDataStorage.Instance.GameData.ItemData.SavedItems.Add(savedItem);
    }

    public override void SaveInventoryItem()
    {
        SavedUseableItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy);
        LocalDataStorage.Instance.PlayerData.SavedInventoryData.SavedItems.Add(savedItem);
    }

    public override void LoadItem(SavedUseableItem item)
    {
        if ((ItemType)item.ItemType == Stats.ItemType)
        {
            item.ApplyToItem(this);
        }
    }
}

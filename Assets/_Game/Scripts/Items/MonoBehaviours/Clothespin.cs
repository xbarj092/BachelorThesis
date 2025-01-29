public class Clothespin : Item
{
    public override void SaveItem()
    {
        SavedItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy);
        LocalDataStorage.Instance.GameData.ItemData.SavedItems.Add(savedItem);
    }

    public override void SaveInventoryItem()
    {
        SavedItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy);
        LocalDataStorage.Instance.PlayerData.SavedInventoryData.SavedItems.Add(savedItem);
    }

    public override void LoadItem(SavedItem item)
    {
        item.ApplyToItem(this);
    }
}

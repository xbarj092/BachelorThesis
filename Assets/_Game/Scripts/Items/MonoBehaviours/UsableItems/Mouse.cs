public class Mouse : UseableItem
{
    public override bool IsInteractable => base.IsInteractable && !Used;

    public override void UseStart()
    {
        base.UseStart();
        Unhighlight();
    }

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
        if ((ItemType)item.ItemType == Stats.ItemType)
        {
            item.ApplyToItem(this);
        }
    }
}

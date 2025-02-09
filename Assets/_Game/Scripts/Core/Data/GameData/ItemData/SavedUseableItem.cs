using System;

[Serializable]
public class SavedUseableItem : SavedItem
{
    public bool Dropped;
    public bool Used;
    public bool Enabled;

    public SavedUseableItem(TransformData itemTransform, int itemType, int uid, bool dropped, bool used, bool enabled) : 
        base(itemTransform, itemType, uid)
    {
        Dropped = dropped;
        Used = used;
        Enabled = enabled;
    }

    public virtual void ApplyToItem(UseableItem item)
    {
        ItemTransform.ApplyToTransform(item.transform);
        item.UID = UID;
        item.Dropped = Dropped;
        item.Used = Used;
        item.gameObject.SetActive(Enabled);
    }
}

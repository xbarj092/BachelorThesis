using System;

[Serializable]
public class SavedItem
{
    public TransformData ItemTransform;

    public int ItemType;
    public int UID;

    public bool Dropped;
    public bool Used;
    public bool Enabled;

    public SavedItem(TransformData itemTransform, int itemType, int uid, bool dropped, bool used, bool enabled)
    {
        ItemTransform = itemTransform;
        ItemType = itemType;
        UID = uid;
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

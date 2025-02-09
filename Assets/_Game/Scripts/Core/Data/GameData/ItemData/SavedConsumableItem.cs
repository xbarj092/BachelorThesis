using System;

[Serializable]
public class SavedConsumableItem : SavedItem
{
    public bool Enabled;

    public SavedConsumableItem(TransformData itemTransform, int itemType, int uID, bool enabled) :
        base(itemTransform, itemType, uID)
    {
        Enabled = enabled;
    }

    public virtual void ApplyToItem(ConsumableItem item)
    {
        ItemTransform.ApplyToTransform(item.transform);
        item.UID = UID;
        item.gameObject.SetActive(Enabled);
    }
}

using System;

[Serializable]
public class SavedConsumableItem : SavedItem
{
    public SavedConsumableItem(TransformData itemTransform, int itemType, int uID) :
        base(itemTransform, itemType, uID)
    {
    }

    public virtual void ApplyToItem(ConsumableItem item)
    {
        ItemTransform.ApplyToTransform(item.transform);
        item.UID = UID;
    }
}

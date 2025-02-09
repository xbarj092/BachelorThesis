using System;

[Serializable]
public class SavedCardboardBoxItem : SavedUseableItem
{
    public bool HasKitten;

    public SavedCardboardBoxItem(TransformData itemTransform, int itemType, int uid, bool dropped, bool used, bool enabled, bool hasKitten) :
        base(itemTransform, itemType, uid, dropped, used, enabled)
    {
        HasKitten = hasKitten;
    }

    public override void ApplyToItem(UseableItem item)
    {
        base.ApplyToItem(item);
        if (item is CardboardBox box)
        {
            box.HasKitten = HasKitten;
        }
    }
}

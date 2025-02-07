using System;

[Serializable]
public class SavedCardboardBoxItem : SavedItem
{
    public bool HasKitten;

    public SavedCardboardBoxItem(TransformData itemTransform, int itemType, int uid, bool dropped, bool used, bool enabled, bool hasKitten) :
        base(itemTransform, itemType, uid, dropped, used, enabled)
    {
        HasKitten = hasKitten;
    }

    public override void ApplyToItem(Item item)
    {
        base.ApplyToItem(item);
        if (item is CardboardBox box)
        {
            box.HasKitten = HasKitten;
        }
    }
}

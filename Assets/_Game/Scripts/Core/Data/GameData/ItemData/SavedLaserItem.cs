using System;

[Serializable]
public class SavedLaserItem : SavedUseableItem
{
    public float Battery;

    public SavedLaserItem(TransformData itemTransform, int itemType, int uid, bool dropped, bool used, bool enabled, float battery) :
        base(itemTransform, itemType, uid, dropped, used, enabled)
    {
        Battery = battery;
    }

    public override void ApplyToItem(UseableItem item)
    {
        base.ApplyToItem(item);
        if (item is Laser laser)
        {
            laser.Battery = Battery;
        }
    }
}

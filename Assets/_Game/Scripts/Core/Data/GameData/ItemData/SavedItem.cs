using System;

[Serializable]
public class SavedItem
{
    public TransformData ItemTransform;

    public int ItemType;
    public int UID;

    public SavedItem(TransformData itemTransform, int itemType, int uID)
    {
        ItemTransform = itemTransform;
        ItemType = itemType;
        UID = uID;
    }
}

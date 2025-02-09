using System;
using System.Collections.Generic;

[Serializable]
public class ItemData
{
    public List<SavedUseableItem> SavedItems = new();
    public List<SavedConsumableItem> SavedConsumables = new();
}

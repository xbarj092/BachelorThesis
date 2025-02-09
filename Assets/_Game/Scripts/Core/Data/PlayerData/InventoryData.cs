using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class InventoryData
{
    public List<UseableItem> ItemsInInventory;
    public int CurrentHighlightIndex;

    public InventoryData(List<UseableItem> items)
    {
        ItemsInInventory = items;
    }

    public bool IsInventoryEmpty()
    {
        return !ItemsInInventory.Where(item => item != null).Any();
    }

    public bool HasRoomInInventory()
    {
        return ItemsInInventory.Any(item => item == null);
    }

    public void RemoveItemFromInventory(UseableItem item)
    {
        if (ItemsInInventory[CurrentHighlightIndex] == item)
        {
            ItemsInInventory[CurrentHighlightIndex] = null;
            DataEvents.OnInventoryDataChangedInvoke(this);
        }
    }
}

[Serializable]
public class SavedInventoryData
{
    public List<SavedItem> SavedItems;
    public int CurrentHighlightIndex;
}

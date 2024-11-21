using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class InventoryData
{
    public List<Item> ItemsInInventory;
    public int CurrentHighlightIndex;

    public InventoryData(List<Item> items)
    {
        ItemsInInventory = items;
    }

    public bool HasRoomInInventory()
    {
        return ItemsInInventory.Any(item => item == null);
    }
}

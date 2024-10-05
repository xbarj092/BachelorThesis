using System;
using System.Collections.Generic;

[Serializable]
public class InventoryData
{
    public List<Item> ItemsInInventory;

    public InventoryData(List<Item> items)
    {
        ItemsInInventory = items;
    }
}

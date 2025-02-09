using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : ConsumableItem
{
    public override void LoadItem(SavedConsumableItem item)
    {
        if ((ConsumableType)item.ItemType == Stats.ConsumableType)
        {
            item.ApplyToItem(this);
        }
    }
}

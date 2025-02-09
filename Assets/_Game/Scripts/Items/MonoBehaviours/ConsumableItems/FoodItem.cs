using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : ConsumableItem
{
    public override void SaveItem()
    {
        SavedUseableItem savedItem = new(new(transform), (int)Stats.ConsumableType, UID, false, false, gameObject.activeInHierarchy);
        LocalDataStorage.Instance.GameData.ItemData.SavedItems.Add(savedItem);
    }

    public override void LoadItem(SavedConsumableItem item)
    {
        if ((ConsumableType)item.ItemType == Stats.ConsumableType)
        {
            item.ApplyToItem(this);
        }
    }
}

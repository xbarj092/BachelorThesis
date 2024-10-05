using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothespinItemStrategy : ItemStrategyBase
{
    public override void Use(Item item)
    {
        Debug.Log("[ClothespinItemStrategy] - Used clothespin!");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[ClothespinItemStrategy] - Picked up clothespin!");
    }
}

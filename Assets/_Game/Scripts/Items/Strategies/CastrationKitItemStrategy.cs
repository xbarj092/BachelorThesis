using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastrationKitItemStrategy : ItemStrategyBase
{
    public override bool CanUse(Item item)
    {
        throw new System.NotImplementedException();
    }

    public override void Use(Item item)
    {
        Debug.Log("[CastrationKitItemStrategy] - Used castration kit!");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[ClothespinItemStrategy] - Picked up castration kit!");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseItemStrategy : ItemStrategyBase
{
    public override void Use(Item item)
    {
        Debug.Log("[MouseItemStrategy] - Used mouse!");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[MouseItemStrategy] - Picked up mouse!");
    }
}

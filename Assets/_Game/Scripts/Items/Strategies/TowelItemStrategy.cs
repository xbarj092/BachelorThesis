using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowelItemStrategy : ItemStrategyBase
{
    public override void Use(Item item)
    {
        Debug.Log("[TowelItemStrategy] - Used towel!");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[TowelItemStrategy] - Picked up towel!");
    }
}

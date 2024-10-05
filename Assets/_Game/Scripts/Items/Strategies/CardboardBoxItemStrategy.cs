using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardboardBoxItemStrategy : ItemStrategyBase
{
    public override void Use(Item item)
    {
        Debug.Log("[CardboardBoxItemStrategy] - Used cardboard box!");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[CardboardBoxItemStrategy] - Picked up cardboard box!");
    }
}

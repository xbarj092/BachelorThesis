using UnityEngine;

public class MouseItemStrategy : ItemStrategyBase
{
    public override void Use(Item item)
    {
        Debug.Log("[MouseItemStrategy] - Used mouse!");
        PlaceOnMousePosition(item);
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[MouseItemStrategy] - Picked up mouse!");
    }
}

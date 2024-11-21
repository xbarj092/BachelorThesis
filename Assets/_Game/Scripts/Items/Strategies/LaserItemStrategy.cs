using UnityEngine;

public class LaserItemStrategy : ItemStrategyBase
{
    public override bool CanUse(Item item)
    {
        throw new System.NotImplementedException();
    }

    public override void Use(Item item)
    {
        Debug.Log("[LaserItemStrategy] - Used laser!");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[LaserItemStrategy] - Picked up laser!");
    }
}

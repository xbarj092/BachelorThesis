using UnityEngine;

public class TowelItemStrategy : ItemStrategyBase
{
    public override bool CanUse(Item item)
    {
        throw new System.NotImplementedException();
    }

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

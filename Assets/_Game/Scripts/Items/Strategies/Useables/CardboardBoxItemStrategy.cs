using UnityEngine;

public class CardboardBoxItemStrategy : UseableItemStrategy
{
    public override bool CanUse(UseableItem item)
    {
        return true;
    }

    public override void Use(UseableItem item)
    {
        Debug.Log("[CardboardBoxItemStrategy] - Used cardboard box!");
        PlaceOnMousePosition(item);
    }

    public override void PickUp(UseableItem item)
    {
        base.PickUp(item);
        Debug.Log("[CardboardBoxItemStrategy] - Picked up cardboard box!");
    }
}

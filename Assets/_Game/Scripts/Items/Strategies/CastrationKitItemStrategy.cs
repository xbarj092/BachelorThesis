using UnityEngine;

public class CastrationKitItemStrategy : ItemStrategyBase
{
    private Kitten _kitten;

    public override bool CanUse(Item item)
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.TryGetComponent(out Kitten kitten))
        {
            _kitten = kitten;
            return true;
        }

        return false;
    }

    public override void Use(Item item)
    {
        _kitten.IsCastrated = true;
        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
        Debug.Log("[CastrationKitItemStrategy] - Used castration kit");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[CastrationKitItemStrategy] - Picked up castration kit!");
    }
}

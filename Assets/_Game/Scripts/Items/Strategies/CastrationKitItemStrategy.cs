using UnityEngine;

public class CastrationKitItemStrategy : ItemStrategyBase
{
    private Kitten _kitten;

    public override bool CanUse(Item item)
    {
        RaycastHit2D hit = Physics2D.Raycast(item.transform.position, Vector2.zero, float.MaxValue, LayerMask.GetMask(GlobalConstants.Layers.KittenInteraction.ToString()));
        if (hit.collider != null)
        {
            Kitten kitten = hit.collider.GetComponentInParent<Kitten>();
            if (kitten != null && !kitten.IsCastrated)
            {
                _kitten = kitten;
                return true;
            }
        }

        return false;
    }

    public override void Use(Item item)
    {
        _kitten.IsCastrated = true;
        _kitten.FocusOnPlayer();
        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
        Debug.Log("[CastrationKitItemStrategy] - Used castration kit");
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[CastrationKitItemStrategy] - Picked up castration kit!");
    }
}

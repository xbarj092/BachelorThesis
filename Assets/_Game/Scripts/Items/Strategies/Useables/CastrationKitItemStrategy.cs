using UnityEngine;

public class CastrationKitItemStrategy : UseableItemStrategy
{
    private Kitten _kitten;

    public override bool CanUse(UseableItem item)
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

    public override void Use(UseableItem item)
    {
        _kitten.IsCastrated = true;
        _kitten.FocusOnPlayer();
        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
        AudioManager.Instance.Play(SoundType.ItemUsed);
    }

    public override void PickUp(UseableItem item)
    {
        base.PickUp(item);
    }
}

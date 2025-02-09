using UnityEngine;

public class ClothespinItemStrategy : ItemStrategyBase
{
    private Kitten _kitten;

    private static DummyMonoBehaviour _coroutineMonoBehaviour;
    private static DummyMonoBehaviour CoroutineMonoBehaviour
    {
        get
        {
            if (_coroutineMonoBehaviour == null)
            {
                GameObject loaderGameObject = new("Coroutine Game Object");
                _coroutineMonoBehaviour = loaderGameObject.AddComponent<DummyMonoBehaviour>();
            }

            return _coroutineMonoBehaviour;
        }
    }

    public override bool CanUse(UseableItem item)
    {
        RaycastHit2D hit = Physics2D.Raycast(item.transform.position, Vector2.zero, float.MaxValue, LayerMask.GetMask(GlobalConstants.Layers.KittenInteraction.ToString()));
        if (hit.collider != null)
        {
            Kitten kitten = hit.collider.GetComponentInParent<Kitten>();
            if (kitten != null && !kitten.IsTrapped)
            {
                _kitten = kitten;
                return true;
            }
        }

        return false;
    }

    public override void Use(UseableItem item)
    {
        _kitten.Trap();
        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
        Debug.Log("[ClothespinItemStrategy] - Used clothespin!");
    }

    public override void PickUp(UseableItem item)
    {
        base.PickUp(item);
        Debug.Log("[ClothespinItemStrategy] - Picked up clothespin!");
    }
}

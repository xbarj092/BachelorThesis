using System.Collections;
using UnityEngine;

public class TowelItemStrategy : ItemStrategyBase
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
        CoroutineMonoBehaviour.StartCoroutine(SetKittenTrapped());
        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
        Debug.Log("[TowelItemStrategy] - Used towel!");
    }

    private IEnumerator SetKittenTrapped()
    {
        _kitten.Trap();
        yield return new WaitForSeconds(5f);
        _kitten.Untrap();
    }

    public override void PickUp(UseableItem item)
    {
        base.PickUp(item);
        Debug.Log("[TowelItemStrategy] - Picked up towel!");
    }
}

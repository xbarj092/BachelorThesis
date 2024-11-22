using System;
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
        CoroutineMonoBehaviour.StartCoroutine(SetKittenTrapped());
        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
        Debug.Log("[TowelItemStrategy] - Used towel!");
    }

    private IEnumerator SetKittenTrapped()
    {
        _kitten.IsTrapped = true;
        yield return new WaitForSeconds(5f);
        _kitten.IsTrapped = false;
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[TowelItemStrategy] - Picked up towel!");
    }
}

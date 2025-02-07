using System.Collections;
using UnityEngine;

public class LaserItemStrategy : ItemStrategyBase
{
    private static DummyMonoBehaviour _monoBehaviour;
    private static DummyMonoBehaviour MonoBehaviour
    {
        get
        {
            if (_monoBehaviour == null)
            {
                GameObject loaderGameObject = new("Coroutine Game Object");
                _monoBehaviour = loaderGameObject.AddComponent<DummyMonoBehaviour>();
            }

            return _monoBehaviour;
        }
    }

    public override bool CanUse(Item item)
    {
        if (((Laser)item).Battery > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Use(Item item)
    {
        MonoBehaviour.StartCoroutine(DepleteBattery(item));
        Debug.Log("[LaserItemStrategy] - Used laser!");
    }

    private IEnumerator DepleteBattery(Item item)
    {
        while (((Laser)item).Battery > 0)
        {
            ((Laser)item).Battery -= 0.01f;
            yield return null;
        }
    }

    public override void PickUp(Item item)
    {
        base.PickUp(item);
        Debug.Log("[LaserItemStrategy] - Picked up laser!");
    }

    public void StopCoroutines()
    {
        MonoBehaviour.StopAllCoroutines();
    }
}

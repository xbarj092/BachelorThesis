using System.Collections;
using UnityEngine;

public class LaserItemStrategy : UseableItemStrategy
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

    public override bool CanUse(UseableItem item)
    {
        if (TutorialManager.Instance.IsTutorialPlaying(TutorialID.Kittens))
        {
            float distance = Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(TutorialManager.Instance.CurrentKittenInRange.transform.position));

            if (distance > 400)
            {
                return false;
            }
        }

        if (((Laser)item).Battery > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Use(UseableItem item)
    {
        AudioManager.Instance.Play(SoundType.ItemUsed);
        MonoBehaviour.StartCoroutine(DepleteBattery(item));
    }

    private IEnumerator DepleteBattery(UseableItem item)
    {
        while (((Laser)item).Battery > 0)
        {
            ((Laser)item).Battery -= 0.01f;
            yield return null;
        }
    }

    public override void PickUp(UseableItem item)
    {
        base.PickUp(item);
    }

    public void StopCoroutines()
    {
        MonoBehaviour.StopAllCoroutines();
    }
}

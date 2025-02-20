using System;
using UnityEngine;

public abstract class TutorialAction : MonoBehaviour
{
    protected TutorialPlayer _tutorialPlayer;

    protected static readonly Vector3 TRANSFORM_POSITION_OFFSET = new(0, 300);

    public event Action OnActionFinished;

    public virtual void Init(TutorialPlayer parentPlayer)
    {
        _tutorialPlayer = parentPlayer;
    }

    public void OnActionFinishedInvoke()
    {
        OnActionFinished?.Invoke();
    }

    protected void InitTutorialObject(Component obj, Vector3 position)
    {
        obj.gameObject.SetActive(true);
        obj.transform.position = position;
    }

    protected virtual T GetComponentReference<T>() where T : Component
    {
        T foundObject = FindObjectOfType<T>();
        if (foundObject == null)
        {
            Debug.LogError($"[{gameObject.name}] Cannot find {nameof(T)}, please investigate!");
        }

        return foundObject;
    }

    public abstract void StartAction();
    public abstract void Exit();
}

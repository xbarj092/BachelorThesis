using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this class as a base for your own action implementation if needed
/// This action includes default behaviour, which is simple "continue on click"
/// </summary>
public abstract class TutorialAction : MonoBehaviour
{
    public event Action OnActionFinished;

    protected TutorialPlayer _tutorialPlayer;

    protected static readonly Vector3 TRANSFORM_POSITION_OFFSET = new(0, 300);

    /// <summary>
    /// Default implementation of Init method, override if you need different behaviour
    /// </summary>
    public virtual void Init(TutorialPlayer parentPlayer)
    {
        _tutorialPlayer = parentPlayer;
    }

    public void OnActionFinishedInvoke()
    {
        OnActionFinished?.Invoke();
    }

    // Use this method to position buttons and cutouts
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

    // Implement followig methods for your own behaviour
    /// <summary>
    /// Called when the action starts
    /// </summary>
    public abstract void StartAction();
    /// <summary>
    /// Called when the action ends
    /// </summary>
    public abstract void Exit();
}

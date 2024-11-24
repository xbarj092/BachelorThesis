using System;
using UnityEngine;

public class InteractionControler : MonoBehaviour
{
    public bool Interactable;
    public bool _interactable
    {
        get => Interactable;
        private set
        {
            Interactable = value;
            if (value == false)
            {
                OnItemOutOfRangeInvoke();
            }
            else
            {
                OnItemInRangeInvoke();
            }
        }
    }

    public event Action OnItemOutOfRange;

    private void OnItemOutOfRangeInvoke()
    {
        OnItemOutOfRange?.Invoke();
    }

    public event Action OnItemInRange;

    private void OnItemInRangeInvoke()
    {
        OnItemInRange?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerInteraction"))
        {
            _interactable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerInteraction"))
        {
            _interactable = false;
        }
    }
}

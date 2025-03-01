using System;
using UnityEngine;

public class MouseInputHandler
{
    private PlayerInteraction _playerInteractions;
    private LayerMask _layerMask;

    public event Action<UseableItem> OnItemPickedUp;
    public event Action<ConsumableItem> OnConsumablePickedUp;
    public event Action OnItemPlaced;
    public event Action<UseableItem> OnItemHighlighted;
    public event Action<ConsumableItem> OnConsumableHighlighted;
    public event Action OnItemUnhighlighted;

    public MouseInputHandler(PlayerInteraction playerInteractions, LayerMask layerMask)
    {
        _playerInteractions = playerInteractions;
        _layerMask = layerMask;
    }

    public void HandleInteraction()
    {
        if (Camera.main != null && ScreenManager.Instance.ActiveGameScreen == null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            HandleMouseClick(mousePosition);
        }
    }

    private void HandleMouseClick(Vector2 mousePosition)
    {
        if (HandlePickUpAction(mousePosition)) return;
        if (HandlePlaceAction()) return;
    }

    private bool HandlePickUpAction(Vector2 mousePosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 2f, _layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Item"))
        {
            UseableItem item = hit.collider.gameObject.GetComponent<UseableItem>();
            if (item != null && item.IsInteractable)
            {
                if (TutorialManager.Instance.IsTutorialPlaying(TutorialID.ItemInteractions))
                {
                    if (TutorialManager.Instance.CurrentItemInRange == item)
                    {
                        TutorialEvents.OnItemPickedUpInvoke();
                    }
                    else
                    {
                        return false;
                    }
                }

                OnItemPickedUp?.Invoke(item);
                return true;
            }
        }
        else if (hit.collider != null && hit.collider.CompareTag("Food"))
        {
            ConsumableItem item = hit.collider.gameObject.GetComponent<ConsumableItem>();
            if (item != null)
            {
                OnConsumablePickedUp?.Invoke(item);
            }
        }
        return false;
    }

    private bool HandlePlaceAction()
    {
        OnItemPlaced?.Invoke();
        return true;
    }

    public void HandleMouseHover(Vector2 mousePosition)
    {
        if (!TutorialManager.Instance.CanHighlightItem)
        {
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 2f, _layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Item"))
        {
            UseableItem item = hit.collider.gameObject.GetComponent<UseableItem>();
            if (item != null)
            {
                if (item.IsInteractable)
                {
                    item.IsHovered = true;
                    OnItemHighlighted?.Invoke(item);
                }
            }
        }
        else if (hit.collider != null && hit.collider.CompareTag("Food"))
        {
            ConsumableItem item = hit.collider.gameObject.GetComponent<ConsumableItem>();
            if (item != null)
            {
                OnConsumableHighlighted?.Invoke(item);
            }
        }
        else
        {
            OnItemUnhighlighted?.Invoke();
        }
    }
}

using System;
using UnityEngine;

public class MouseInputHandler
{
    private PlayerInteraction _playerInteractions;
    private LayerMask _layerMask;

    public event Action<Item> OnItemPickedUp;
    public event Action OnItemPlaced;
    public event Action<Item> OnItemHighlighted;
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
            Item item = hit.collider.gameObject.GetComponent<Item>();
            if (item != null && item.IsInteractable)
            {
                OnItemPickedUp?.Invoke(item);
                return true;
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
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 2f, _layerMask);
        if (hit.collider != null && hit.collider.CompareTag("Item"))
        {
            Item item = hit.collider.gameObject.GetComponent<Item>();
            if (item != null && item.IsInteractable)
            {
                OnItemHighlighted?.Invoke(item);
            }
        }
        else
        {
            OnItemUnhighlighted?.Invoke();
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private LayerMask _mapLayer;
    [SerializeField] private float _maxRange = 5f;
    [SerializeField] private float _placementRadius = 0.35f;
    [SerializeField] private SpriteRenderer _interactionZone;
    [SerializeField] private Player _player;

    private Item _highlightedItem;
    private Item _carryingItem;
    private GameObject _ghostItem;
    private SpriteRenderer _ghostRenderer;

    private MouseInputHandler _mouseInputHandler;
    private InputAction _leftClickAction;
    private InputAction _rightClickAction;

    private bool _isUsingItem = false;

    private void Awake()
    {
        _mouseInputHandler = new MouseInputHandler(this, _interactLayer);
        InitializeInputActions();
    }

    private void InitializeInputActions()
    {
        _leftClickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/leftButton");
        _rightClickAction = new InputAction(type: InputActionType.Button, binding: "<Mouse>/rightButton");
    }

    private void OnEnable()
    {
        _mouseInputHandler.OnItemPickedUp += PickUpItem;
        _mouseInputHandler.OnItemPlaced += PlaceItem;
        _mouseInputHandler.OnItemHighlighted += HighlightItem;
        _mouseInputHandler.OnItemUnhighlighted += UnhighlightItem;

        _leftClickAction.performed += context => PickUpFromGroundOrUse();
        _leftClickAction.canceled += context => StopUsingItem();
        _rightClickAction.performed += context => PlaceInInventoryOrPickUpFromInventory();

        EnableActions();
    }

    private void OnDisable()
    {
        _mouseInputHandler.OnItemPickedUp -= PickUpItem;
        _mouseInputHandler.OnItemPlaced -= PlaceItem;
        _mouseInputHandler.OnItemHighlighted -= HighlightItem;
        _mouseInputHandler.OnItemUnhighlighted -= UnhighlightItem;

        _leftClickAction.performed -= context => PickUpFromGroundOrUse();
        _leftClickAction.canceled -= context => StopUsingItem();
        _rightClickAction.performed -= context => PlaceInInventoryOrPickUpFromInventory();

        DisableActions();
    }

    private void EnableActions()
    {
        _leftClickAction.Enable();
        _rightClickAction.Enable();
    }

    private void DisableActions()
    {
        _leftClickAction.Disable();
        _rightClickAction.Disable();
    }

    private void Update()
    {
        if (Camera.main != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mouseInputHandler.HandleMouseHover(mousePosition);

            if (_ghostItem != null)
            {
                SnapGhostToMousePosition();
                _ghostRenderer.material.SetInt("_Outlined", CanItemBePlaced() ? 0 : 1);
            }
        }
    }

    // Actions

    private void PickUpFromGroundOrUse()
    {
        if (_carryingItem == null)
        {
            _mouseInputHandler.HandleInteraction();
        }
        else
        {
            UseItem();
        }
    }

    private void PlaceInInventoryOrPickUpFromInventory()
    {
        if (_carryingItem != null)
        {
            PlaceInInventory();
        }
        else
        {
            PickUpFromInventory();
        }
    }

    private void PickUpFromInventory()
    {
        InventoryData inventory = LocalDataStorage.Instance.PlayerData.InventoryData;
        Item item = inventory.ItemsInInventory[inventory.CurrentHighlightIndex];
        if (item != null)
        {
            PickUpItem(item);
        }
    }

    private void PlaceInInventory()
    {
        InventoryData inventory = LocalDataStorage.Instance.PlayerData.InventoryData;
        if (!LocalDataStorage.Instance.PlayerData.InventoryData.ItemsInInventory.Contains(_carryingItem))
        {
            if (!inventory.HasRoomInInventory())
            {
                Debug.LogWarning("Inventory is full");
                return;
            }

            _player.PickupItem(_carryingItem.gameObject);
        }

        _carryingItem.IsPickedUp(false);
        DestroyGhostItem();
    }

    private void OnItemDrop()
    {
        InventoryData inventoryData = LocalDataStorage.Instance.PlayerData.InventoryData;
        Item item = _carryingItem == null ? inventoryData.ItemsInInventory[inventoryData.CurrentHighlightIndex] : _carryingItem;

        item.transform.position = transform.position;
        item.gameObject.SetActive(true);
        item.Dropped = true;

        if (_carryingItem != null)
        {
            DestroyGhostItem();
        }

        inventoryData.ItemsInInventory[inventoryData.CurrentHighlightIndex] = null;
        LocalDataStorage.Instance.PlayerData.InventoryData = inventoryData;
    }

    private void UseItem()
    {
        if (_carryingItem == null || !_carryingItem.CanUse())
        {
            return;
        }

        switch (_carryingItem.UsageType)
        {
            case ItemUsageType.SingleUse:
                _carryingItem.UseStart();
                break;

            case ItemUsageType.HoldToUse:
                _isUsingItem = true;
                _carryingItem.UseStart();
                break;
        }

        DestroyGhostItem();
    }

    private void StopUsingItem()
    {
        if (_carryingItem == null || !_isUsingItem)
        {
            return;
        }

        _isUsingItem = false;
        _carryingItem.UseStop();
    }

    // Helpers

    private void PlaceItem()
    {
        if (_carryingItem == null || _ghostItem == null)
        {
            return;
        }

        if (!CanItemBePlaced())
        {
            Debug.LogWarning("Cannot place item here!");
            return;
        }

        _carryingItem.transform.position = _ghostItem.transform.position;
        _carryingItem.gameObject.SetActive(true);
        _carryingItem.IsPickedUp(false);

        DestroyGhostItem();
    }

    private void PickUpItem(Item item)
    {
        if (_carryingItem != null)
        {
            return;
        }

        _carryingItem = item;
        StartCoroutine(HideItemAndCreateGhost(item));
    }

    private IEnumerator HideItemAndCreateGhost(Item item)
    {
        yield return null;
        item.gameObject.SetActive(false);

        _ghostItem = Instantiate(item.GetGhostItem(), transform);
        _ghostRenderer = _ghostItem.GetComponent<SpriteRenderer>();
    }

    private void HighlightItem(Item item)
    {
        if (_highlightedItem == null)
        {
            _highlightedItem = item;
            _highlightedItem.Highlight();
        }
    }

    private void UnhighlightItem()
    {
        if (_highlightedItem != null)
        {
            _highlightedItem.Unhighlight();
            _highlightedItem = null;
        }
    }

    private void SnapGhostToMousePosition()
    {
        if (Camera.main == null)
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 validPosition = GetValidPlacementPosition(transform.position, mousePosition);
        _ghostItem.transform.position = validPosition;
    }

    private bool CanItemBePlaced()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_ghostItem.transform.position, _placementRadius, _interactLayer);
        return colliders.Length == 0;
    }

    private void DestroyGhostItem()
    {
        Destroy(_ghostItem);
        _carryingItem = null;
    }

    public Vector3 GetValidPlacementPosition(Vector3 playerPosition, Vector3 mousePosition)
    {
        Vector3 direction = mousePosition - playerPosition;
        float distance = Mathf.Min(direction.magnitude, _maxRange);
        direction.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(playerPosition, direction, distance, _mapLayer);
        return hit.collider ? (Vector3)hit.point - direction * 0.1f : playerPosition + direction * distance;
    }
}

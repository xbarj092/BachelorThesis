using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private InputActionReference _leftClick;
    [SerializeField] private InputActionReference _rightClick;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private LayerMask _mapLayer;
    [SerializeField] private float _maxRange = 5f;
    [SerializeField] private SpriteRenderer _interactionZone;
    [SerializeField] private Player _player;

    private UseableItem _highlightedItem;
    private UseableItem _carryingItem;
    private GameObject _ghostItem;
    private SpriteRenderer _ghostRenderer;

    private KeyboardInputHandler _keyboardInputHandler = new();
    private MouseInputHandler _mouseInputHandler;
    private InputAction _leftClickAction;
    private InputAction _rightClickAction;

    private float _mouseScroll;
    private bool _isUsingItem = false;

    private void Awake()
    {
        _mouseInputHandler = new MouseInputHandler(this, _interactLayer);

        InitializeInputActions();
    }

    private void InitializeInputActions()
    {
        _leftClickAction = _leftClick.action;
        _rightClickAction = _rightClick.action;
    }

    private void OnEnable()
    {
        _mouseInputHandler.OnItemPickedUp += PickUpItem;
        _mouseInputHandler.OnItemPlaced += PlaceItem;
        _mouseInputHandler.OnItemHighlighted += HighlightItem;
        _mouseInputHandler.OnItemUnhighlighted += UnhighlightItem;

        _leftClickAction.performed += PickUpFromGroundOrUse;
        _leftClickAction.canceled += StopUsingItem;
        _rightClickAction.performed += PlaceInInventoryOrPickUpFromInventory;
    }

    private void OnDisable()
    {
        _mouseInputHandler.OnItemPickedUp -= PickUpItem;
        _mouseInputHandler.OnItemPlaced -= PlaceItem;
        _mouseInputHandler.OnItemHighlighted -= HighlightItem;
        _mouseInputHandler.OnItemUnhighlighted -= UnhighlightItem;

        _leftClickAction.performed -= PickUpFromGroundOrUse;
        _leftClickAction.canceled -= StopUsingItem;
        _rightClickAction.performed -= PlaceInInventoryOrPickUpFromInventory;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _keyboardInputHandler.HandleInteraction();
        }

        if (ScreenManager.Instance.ActiveGameScreen != null)
        {
            return;
        }

        HandleMouseWheelInput();
    }

    private void FixedUpdate()
    {
        if (ScreenManager.Instance.ActiveGameScreen != null)
        {
            return;
        }

        if (Camera.main != null)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mouseInputHandler.HandleMouseHover(mousePosition);

            if (_highlightedItem != null && !CanSeeItem(_highlightedItem.gameObject))
            {
                UnhighlightItem();
            }

            if (_ghostItem != null)
            {
                SnapGhostToMousePosition();
                _ghostRenderer.material.SetInt("_Outlined", CanItemBePlaced() ? 1 : 0);
                _ghostRenderer.color = new Color(_ghostRenderer.color.r, _ghostRenderer.color.g, _ghostRenderer.color.b, CanItemBePlaced() ? 1 : 0.4f);
                _ghostItem.transform.rotation = Quaternion.identity;
            }
        }
    }

    public void OnMouseScroll(InputValue inputValue)
    {
        _mouseScroll = inputValue.Get<float>();
    }

    private void HandleMouseWheelInput()
    {
        if (_mouseScroll > 0)
        {
            ChangeSelectedItem(-1);
        }
        else if (_mouseScroll < 0)
        {
            ChangeSelectedItem(1);
        }
    }

    private void ChangeSelectedItem(int direction)
    {
        InventoryData inventoryData = LocalDataStorage.Instance.PlayerData.InventoryData;

        if (inventoryData.ItemsInInventory.Count == 0)
        {
            Debug.LogWarning("[PlayerInteraction] - inventory items are empty");
            return;
        }

        int nextHighlight = LocalDataStorage.Instance.PlayerData.InventoryData.CurrentHighlightIndex;
        for (int i = 0; i < inventoryData.ItemsInInventory.Count; i++)
        {
            nextHighlight = (nextHighlight + direction + inventoryData.ItemsInInventory.Count) % inventoryData.ItemsInInventory.Count;
            if (inventoryData.ItemsInInventory[nextHighlight] != null)
            {
                break;
            }
        }

        if (inventoryData.ItemsInInventory[nextHighlight] == null)
        {
            Debug.LogWarning("[PlayerInteraction] - next highlight is null");
            return;
        }

        UpdateGhostOrCarryingItemVisuals(inventoryData.ItemsInInventory[nextHighlight]);

        inventoryData.CurrentHighlightIndex = nextHighlight;
        LocalDataStorage.Instance.PlayerData.InventoryData = inventoryData;
    }

    private void UpdateGhostOrCarryingItemVisuals(UseableItem selectedItem)
    {
        if (selectedItem == null)
        {
            return;
        }

        if (_carryingItem != null)
        {
            InventoryData inventoryData = LocalDataStorage.Instance.PlayerData.InventoryData;
            if (!inventoryData.ItemsInInventory.Contains(_carryingItem))
            {
                if (inventoryData.HasRoomInInventory())
                {
                    PlaceInInventory();
                }
                else
                {
                    _carryingItem.transform.position = transform.position;
                    _carryingItem.gameObject.SetActive(true);
                    _carryingItem.Dropped = true;
                    _carryingItem.IsPickedUp(false);
                }
            }

            if (_carryingItem is Laser laser)
            {
                laser.OnBatteryChanged -= OnBatteryChanged;
            }

            if (_isUsingItem)
            {
                StopUsingItem(default);
            }

            DestroyGhostItem();
            _carryingItem = selectedItem;
            HideItemAndCreateGhost(selectedItem);
        }

        if (_highlightedItem != null)
        {
            _highlightedItem.Unhighlight();
        }

        if (selectedItem != null)
        {
            _highlightedItem = selectedItem;
            _highlightedItem.Highlight();
        }
    }

    private void PickUpFromGroundOrUse(InputAction.CallbackContext context)
    {
        if (ScreenManager.Instance.ActiveGameScreen != null)
        {
            return;
        }

        if (_carryingItem == null)
        {
            _mouseInputHandler.HandleInteraction();
        }
        else
        {
            UseItem();
        }
    }

    private void PlaceInInventoryOrPickUpFromInventory(InputAction.CallbackContext context)
    {
        if (ScreenManager.Instance.ActiveGameScreen != null)
        {
            return;
        }

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
        UseableItem item = inventory.ItemsInInventory[inventory.CurrentHighlightIndex];
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

        if (_carryingItem is Laser laser)
        {
            laser.OnBatteryChanged -= OnBatteryChanged;
        }

        if (_isUsingItem)
        {
            StopUsingItem(default);
        }

        _carryingItem.IsPickedUp(false);
        DestroyGhostItem();
    }

    private void OnItemDrop()
    {
        InventoryData inventoryData = LocalDataStorage.Instance.PlayerData.InventoryData;
        UseableItem item = _carryingItem == null ? inventoryData.ItemsInInventory[inventoryData.CurrentHighlightIndex] : _carryingItem;
        if (item == null)
        {
            return;
        }

        if (_isUsingItem)
        {
            StopUsingItem(default);
        }

        if (item is Laser laser)
        {
            laser.OnBatteryChanged -= OnBatteryChanged;
        }

        item.transform.position = transform.position;
        item.gameObject.SetActive(true);
        item.IsPickedUp(false);
        item.Dropped = true;

        if (inventoryData.ItemsInInventory.Contains(item))
        {
            inventoryData.ItemsInInventory[inventoryData.CurrentHighlightIndex] = null;
        }

        LocalDataStorage.Instance.PlayerData.InventoryData = inventoryData;
        if (_carryingItem != null)
        {
            DestroyGhostItem();
            PickUpFromInventory();
        }
    }

    private void UseItem()
    {
        if (_carryingItem == null || !_carryingItem.CanUse())
        {
            return;
        }

        UGSAnalyticsManager.Instance.RecordItemUsed(_carryingItem.Stats.ItemType.ToString(), LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
        _carryingItem.UseStart();

        switch (_carryingItem.UsageType)
        {
            case ItemUsageType.SingleUse:
                DestroyGhostItem();
                PickUpFromInventory();
                break;
            case ItemUsageType.HoldToUse:
                _isUsingItem = true;
                _ghostItem.SetActive(false);
                break;
        }
    }

    private void StopUsingItem(InputAction.CallbackContext context)
    {
        if (ScreenManager.Instance.ActiveGameScreen != null)
        {
            return;
        }

        if (_carryingItem == null || !_isUsingItem)
        {
            return;
        }

        _isUsingItem = false;
        _carryingItem.UseStop();
        _ghostItem.SetActive(true);
    }

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

    private void PickUpItem(UseableItem item)
    {
        if (_carryingItem != null)
        {
            return;
        }

        item.IsPickedUp(true);
        _carryingItem = item;
        HideItemAndCreateGhost(item);
    }

    private void HideItemAndCreateGhost(UseableItem item)
    {
        item.gameObject.SetActive(false);
        item.IsPickedUp(true);

        if (item.Stats.ItemType == ItemType.Laser)
        {
            ((Laser)item).OnBatteryChanged += OnBatteryChanged;
        }

        _ghostItem = Instantiate(item.GetGhostItem(), transform.parent);
        _ghostRenderer = _ghostItem.GetComponent<SpriteRenderer>();
    }

    private void OnBatteryChanged(float battery)
    {
        if (battery <= 0)
        {
            StopUsingItem(default);
            ((Laser)_carryingItem).OnBatteryChanged -= OnBatteryChanged;
            LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(_carryingItem);
            DestroyGhostItem();
            PickUpFromInventory();
        }
    }

    private void HighlightItem(UseableItem item)
    {
        if (_highlightedItem == null && CanSeeItem(item.gameObject))
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
        _carryingItem.transform.position = validPosition;
    }

    private bool CanSeeItem(GameObject item)
    {
        Vector2 itemPosition = item.transform.position;
        Vector2 playerPosition = transform.position;
        Vector2 direction = itemPosition - playerPosition;

        float distance = Vector2.Distance(transform.position, itemPosition);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, _mapLayer);
        return hit.collider == null;
    }

    private bool CanItemBePlaced()
    {
        if (_ghostItem.CompareTag(GlobalConstants.Tags.InteractableGhost.ToString()))
        {
            RaycastHit2D hit = Physics2D.Raycast(_ghostItem.transform.position, Vector2.zero, 2f, LayerMask.GetMask(GlobalConstants.Layers.KittenInteraction.ToString()));
            if (hit.collider != null)
            {
                Kitten kitten = hit.collider.GetComponentInParent<Kitten>();
                if (kitten != null)
                {
                    if (_carryingItem.Stats.ItemType == ItemType.Towel || _carryingItem.Stats.ItemType == ItemType.Clothespin)
                    {
                        return !kitten.IsTrapped;
                    }
                    else if (_carryingItem.Stats.ItemType == ItemType.CastrationKit)
                    {
                        return !kitten.IsCastrated;
                    }

                    return true;
                }
            }
        }

        return false;
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

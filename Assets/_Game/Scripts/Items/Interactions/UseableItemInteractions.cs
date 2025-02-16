using UnityEngine;

public class UseableItemInteractions : ItemInteractions<UseableItem, UseableItemSO, SavedUseableItem>
{
    private void OnEnable()
    {
        DataEvents.OnInventoryDataChanged += OnInventoryDataChanged;
    }

    private void OnDisable()
    {
        DataEvents.OnInventoryDataChanged -= OnInventoryDataChanged;
    }

    private void OnInventoryDataChanged(InventoryData data)
    {
        if (data.HasRoomInInventory())
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _collider.radius * transform.localScale.x);
            foreach (Collider2D collider in colliders)
            {
                if (collider.TryGetComponent(out Player player))
                {
                    if (gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()) && _item.IsInteractable)
                    {
                        _item.PickUp();
                        UGSAnalyticsManager.Instance.RecordItemPickedUp(_item.Stats.ItemType.ToString(), LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
                    }
                }
            }
        }
    }

    protected override void HandlePlayerCollisionEnter(Player player, Collider2D collision)
    {
        if (_item.Used && _item.Stats.ItemType != ItemType.CardboardBox || _item is CardboardBox box && box.HasKitten)
        {
            return;
        }

        if (LocalDataStorage.Instance.PlayerData.InventoryData.HasRoomInInventory())
        {
            _item.PickUp();
            UGSAnalyticsManager.Instance.RecordItemPickedUp(_item.Stats.ItemType.ToString(), LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
        }
    }

    private void HandlePlayerCollisionExit(Player player)
    {
        if (gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()))
        {
            GetComponent<UseableItem>().Dropped = false;
        }
    }

    private void HandleKittenCollisionEnter(Kitten kitten)
    {
        if (_item == null || !_item.Used)
        {
            return;
        }

        if (_item != null && _item is CardboardBox box && !box.HasKitten && !kitten.IsTrapped && !kitten.IsDead && gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()))
        {
            box.SetHasKitten(true);
            kitten.Trap(true);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Kitten kitten))
        {
            HandleKittenCollisionEnter(kitten);
        }

        base.OnTriggerEnter2D(collision);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            HandlePlayerCollisionExit(player);
        }

        base.OnTriggerExit2D(collision);
    }
}

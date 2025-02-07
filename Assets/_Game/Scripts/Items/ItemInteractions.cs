using UnityEngine;

public class ItemInteractions : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _collider;
    [SerializeField] private Item _item;

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
                        player.PickupItem(gameObject, true);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Kitten kitten))
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

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            if (gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()))
            {
                if (_item.Used && _item.Stats.ItemType != ItemType.CardboardBox || _item is CardboardBox box && box.HasKitten)
                {
                    return;
                }

                player.PickupItem(gameObject, true);
            }
            else if (gameObject.CompareTag(GlobalConstants.Tags.Food.ToString()))
            {
                player.PickupFood(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            if (gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()))
            {
                GetComponent<Item>().Dropped = false;
            }
        }
    }
}

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
                    if (gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()))
                    {
                        player.PickupItem(gameObject);
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

            if (_item?.Stats.ItemType == ItemType.CardboardBox && gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()))
            {
                kitten.IsTrapped = true;
            }
        }

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            if (gameObject.CompareTag(GlobalConstants.Tags.Item.ToString()))
            {
                player.PickupItem(gameObject);
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

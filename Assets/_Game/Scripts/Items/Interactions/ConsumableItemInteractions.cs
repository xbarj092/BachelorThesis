using UnityEngine;

public class ConsumableItemInteractions : ItemInteractions<ConsumableItem, ConsumableItemSO, SavedConsumableItem>
{
    protected override void HandlePlayerCollisionEnter(Player player, Collider2D collision)
    {
        _item.PickUp();
    }
}

using UnityEngine;

public class ConsumableItemInteractions : ItemInteractions<ConsumableItem, ConsumableItemSO>
{
    protected override void HandlePlayerCollisionEnter(Player player, Collider2D collision)
    {
        player.PickupFood(gameObject);
    }
}

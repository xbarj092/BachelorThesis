using UnityEngine;

public abstract class ItemStrategyBase : IItemStrategy
{
    public abstract bool CanUse(Item item);
    public abstract void Use(Item item);
    public virtual void PickUp(Item item)
    {
        item.gameObject.SetActive(false);
    }

    protected void PlaceOnMousePosition(Item item)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        PlayerInteraction player = GameObject.FindFirstObjectByType<PlayerInteraction>();
        Vector3 playerPosition = player.transform.position;

        Vector3 validPosition = player.GetValidPlacementPosition(playerPosition, mousePosition);

        item.gameObject.transform.position = validPosition;
        item.gameObject.SetActive(true);

        LocalDataStorage.Instance.PlayerData.InventoryData.RemoveItemFromInventory(item);
    }
}

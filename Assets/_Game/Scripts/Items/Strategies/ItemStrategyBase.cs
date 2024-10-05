using UnityEngine;

public abstract class ItemStrategyBase : IItemStrategy
{
    public abstract void Use(Item item);
    public virtual void PickUp(Item item)
    {
        item.gameObject.SetActive(false);
    }

    protected void PlaceOnMousePosition(Item item)
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        Object.Instantiate(item, worldPosition, Quaternion.identity);
    }
}

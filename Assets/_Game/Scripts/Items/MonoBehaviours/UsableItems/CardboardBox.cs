using UnityEngine;

public class CardboardBox : UseableItem
{
    [SerializeField] private Sprite _openedBox;
    [SerializeField] private Sprite _occupiedBox;

    public bool HasKitten;
    public override bool IsInteractable => !HasKitten && base.IsInteractable;

    public override void UseStart()
    {
        _spriteRenderer.sprite = _openedBox;
        Vector2 itemPosition = transform.position;
        float overlapRadius = 0.25f;

        Collider2D playerCollider = Physics2D.OverlapCircle(itemPosition, overlapRadius, LayerMask.GetMask(GlobalConstants.Layers.Player.ToString()));

        if (playerCollider != null && playerCollider.CompareTag(GlobalConstants.Tags.Player.ToString()))
        {
            Dropped = true;
        }

        base.UseStart();
    }

    public void SetHasKitten(bool hasKitten)
    {
        HasKitten = true;
        _spriteRenderer.sprite = _occupiedBox;
    }

    public override void SaveItem()
    {
        SavedCardboardBoxItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy, HasKitten);
        LocalDataStorage.Instance.GameData.ItemData.SavedItems.Add(savedItem);
    }

    public override void SaveInventoryItem()
    {
        SavedCardboardBoxItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy, HasKitten);
        LocalDataStorage.Instance.PlayerData.SavedInventoryData.SavedItems.Add(savedItem);
    }

    public override void LoadItem(SavedItem item)
    {
        if ((ItemType)item.ItemType == Stats.ItemType)
        {
            ((SavedCardboardBoxItem)item).ApplyToItem(this);
        }
    }
}

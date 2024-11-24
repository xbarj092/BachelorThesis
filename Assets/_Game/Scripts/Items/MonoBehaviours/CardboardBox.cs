using UnityEngine;

public class CardboardBox : Item
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
}

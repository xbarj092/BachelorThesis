using UnityEngine;

public abstract class ItemInteractions<T, U, V> : MonoBehaviour 
    where T : Item<U, V> 
    where U : ItemBaseSO
    where V : SavedItem
{
    [SerializeField] protected CircleCollider2D _collider;
    [SerializeField] protected T _item;

    protected abstract void HandlePlayerCollisionEnter(Player player, Collider2D collision);

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            HandlePlayerCollisionEnter(player, collision);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
    }
}

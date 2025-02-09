using UnityEngine;

public abstract class Item<T, U> : MonoBehaviour 
    where T : ItemBaseSO
    where U : SavedItem
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    [field: SerializeField] public T Stats { get; private set; }

    public int UID;

    protected StrategyFactory _strategyFactory = new();

    public abstract void SaveItem();
    public abstract void LoadItem(U item);
}

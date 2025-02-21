using UnityEngine;

public abstract class Item<T, U> : MonoBehaviour 
    where T : ItemBaseSO
    where U : SavedItem
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _outlineMaterial;

    [field: SerializeField] public T Stats { get; private set; }

    public bool IsHovered;
    public bool Highlighting;
    public int UID;

    protected StrategyFactory _strategyFactory = new();

    public void Highlight()
    {
        if (!Highlighting)
        {
            Highlighting = true;
            _spriteRenderer.material = _outlineMaterial;
            _spriteRenderer.material.SetInt("_Outlined", 1);
        }
    }

    public void Unhighlight()
    {
        IsHovered = false;
        if (Highlighting)
        {
            Highlighting = false;
            _spriteRenderer.material.SetInt("_Outlined", 0);
            _spriteRenderer.material = _defaultMaterial;
        }
    }

    public abstract void PickUp();
    public abstract void SaveItem();
    public abstract void LoadItem(U item);
}

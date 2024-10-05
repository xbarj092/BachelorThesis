using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [field: SerializeField] public UseableItem Stats { get; private set; }

    private ItemStrategyBase _strategy;
    private StrategyFactory _strategyFactory = new();

    private void Awake()
    {
        _strategy = _strategyFactory.CreateStrategy(Stats.ItemType);
        _spriteRenderer.sprite = Stats.Sprite;
    }

    public virtual void Use()
    {
        _strategy.Use(this);
    }

    public virtual void PickUp()
    {
        _strategy.PickUp(this);
    }
}

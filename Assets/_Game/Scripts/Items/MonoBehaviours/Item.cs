using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [field: SerializeField] public UseableItem Stats { get; private set; }

    [SerializeField] private GameObject _ghostItem;
    [SerializeField] private InteractionControler _interaction;

    private ItemStrategyBase _strategy;
    private StrategyFactory _strategyFactory = new();
    private bool _pickedUp;

    public ItemUsageType UsageType;
    public bool IsInteractable => _interaction.Interactable;

    public bool Dropped;

    public event Action OnItemOutOfRange;
    private void OnItemOutOfRangeInvoke()
    {
        OnItemOutOfRange?.Invoke();
    }

    private void Awake()
    {
        _strategy = _strategyFactory.CreateStrategy(Stats.ItemType);
        _spriteRenderer.sprite = Stats.Sprite;
    }

    protected virtual void OnEnable()
    {
        _interaction.OnItemOutOfRange += OnItemOutOfRangeInvoke;
    }

    protected virtual void OnDisable()
    {
        _interaction.OnItemOutOfRange -= OnItemOutOfRangeInvoke;
    }

    public bool CanUse()
    {
        return _strategy.CanUse(this);
    }

    public virtual void UseStart()
    {
        _strategy.Use(this);
    }

    public virtual void UseStop()
    {
        // TODO
    }

    public virtual void PickUp()
    {
        _strategy.PickUp(this);
    }

    public void Highlight()
    {

    }

    public void Unhighlight()
    {

    }

    public GameObject GetGhostItem()
    {
        return _ghostItem;
    }

    public void IsPickedUp(bool pickedUp)
    {
        _pickedUp = pickedUp;
    }
}

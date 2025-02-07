using System;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [field: SerializeField] public UseableItem Stats { get; private set; }

    [SerializeField] private GameObject _ghostItem;
    [SerializeField] private InteractionControler _interaction;

    private ItemStrategyBase _strategy;
    private StrategyFactory _strategyFactory = new();
    protected bool _pickedUp;

    public ItemUsageType UsageType;
    public virtual bool IsInteractable => _interaction.Interactable;

    public bool IsHovered;
    public bool Highlighting;
    public bool Dropped;
    public bool Used;

    public int UID;

    private void OnItemInRangeInvoke()
    {
        if (IsHovered && IsInteractable)
        {
            Highlight();
            IsHovered = false;
        }
    }

    public event Action OnItemOutOfRange;
    private void OnItemOutOfRangeInvoke()
    {
        Unhighlight();
        OnItemOutOfRange?.Invoke();
    }

    protected virtual void Awake()
    {
        _strategy = _strategyFactory.CreateStrategy(Stats.ItemType);
        _spriteRenderer.sprite = Stats.Sprite;
        Unhighlight();
    }

    protected virtual void OnEnable()
    {
        _interaction.OnItemOutOfRange += OnItemOutOfRangeInvoke;
        _interaction.OnItemInRange += OnItemInRangeInvoke;
    }

    protected virtual void OnDisable()
    {
        _interaction.OnItemOutOfRange -= OnItemOutOfRangeInvoke;
        _interaction.OnItemInRange -= OnItemInRangeInvoke;
    }

    public virtual bool CanUse()
    {
        return _strategy.CanUse(this);
    }

    public virtual void UseStart()
    {
        Used = true;
        _strategy.Use(this);
    }

    public virtual void UseStop()
    {
        Used = false;
        ((LaserItemStrategy)_strategy).StopCoroutines();
    }

    public virtual void PickUp()
    {
        _strategy.PickUp(this);
    }

    public void Highlight()
    {
        if (!Highlighting)
        {
            Highlighting = true;
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
        }
    }

    public GameObject GetGhostItem()
    {
        return _ghostItem;
    }

    public void IsPickedUp(bool pickedUp)
    {
        _pickedUp = pickedUp;
    }

    public abstract void SaveItem();
    public abstract void SaveInventoryItem();
    public abstract void LoadItem(SavedItem item);
}

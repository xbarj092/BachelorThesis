using System;
using UnityEngine;

public abstract class UseableItem : Item<UseableItemSO, SavedUseableItem>
{
    [SerializeField] private GameObject _ghostItem;
    [SerializeField] private InteractionControler _interaction;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _outlineMaterial;

    protected bool _pickedUp;

    public ItemUsageType UsageType;
    public virtual bool IsInteractable => _interaction.Interactable;

    public bool IsHovered;
    public bool Highlighting;
    public bool Dropped;
    public bool Used;

    private UseableItemStrategy _strategy;

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

    public override void PickUp()
    {
        _strategy.PickUp(this);
    }

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

    public GameObject GetGhostItem()
    {
        return _ghostItem;
    }

    public void IsPickedUp(bool pickedUp)
    {
        _pickedUp = pickedUp;
    }

    public override abstract void SaveItem();
    public abstract void SaveInventoryItem();
    public override abstract void LoadItem(SavedUseableItem item);
}

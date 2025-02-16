using System.Collections.Generic;
using UnityEngine;

public class CollectibleGameScreen<T> : BaseScreen where T : ICollectible
{
    [SerializeField] private CollectibleDetailPopup _collectibleDetailPopup;
    [SerializeField] private List<T> _collectibles = new();
    [SerializeField] private CollectibleSlot _collectibleSlotPrefab;
    [SerializeField] private Transform _spawnTransform;

    private List<CollectibleSlot> _slots = new();

    private void Start()
    {
        SetUpSlots();
    }

    private void OnDisable()
    {
        foreach (CollectibleSlot slot in _slots)
        {
            slot.OnClick -= OnClick;
        }
    }

    private void SetUpSlots()
    {
        foreach (ICollectible collectible in _collectibles)
        {
            CollectibleSlot slot = Instantiate(_collectibleSlotPrefab, _spawnTransform);
            slot.OnClick += OnClick;
            slot.Init(collectible);
            _slots.Add(slot);
        }
    }

    private void OnClick(ICollectible collectible)
    {
        CollectibleDetailPopup popup = Instantiate(_collectibleDetailPopup, transform);
        popup.Init(collectible);
    }
}

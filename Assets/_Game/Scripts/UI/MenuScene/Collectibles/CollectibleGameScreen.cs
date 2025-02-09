using System.Collections.Generic;
using UnityEngine;

public class CollectibleGameScreen<T> : BaseScreen where T : ICollectible
{
    [SerializeField] private List<T> _collectibles = new();
    [SerializeField] private CollectibleSlot _collectibleSlotPrefab;
    [SerializeField] private Transform _spawnTransform;

    private void Start()
    {
        SetUpSlots();
    }

    private void SetUpSlots()
    {
        foreach (ICollectible collectible in _collectibles)
        {
            CollectibleSlot slot = Instantiate(_collectibleSlotPrefab, _spawnTransform);
            slot.Init(collectible);
        }
    }
}

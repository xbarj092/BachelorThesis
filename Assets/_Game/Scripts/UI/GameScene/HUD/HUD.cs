using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private List<Image> _foodUnits;
    [SerializeField] private StatusEffects _statusEffects;
    [SerializeField] private List<ItemSlot> _itemSlots;
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private FloatingText _foodChangedText;

    private int _currentHighlightIndex;

    private void Start()
    {
        InvokeRepeating(nameof(UpdateTimer), 0, 1);
    }

    private void OnEnable()
    {
        DataEvents.OnPlayerStatsChanged += OnPlayerStatsChanged;
        DataEvents.OnInventoryDataChanged += ChangeInventoryItems;

        GameEvents.OnFoodStateChanged += OnFoodChanged;

        if (_itemSlots.Count > 0)
        {
            HighlightItem(_itemSlots.FindIndex(slot => slot.Occupied));
        }
    }

    private void OnDisable()
    {
        DataEvents.OnPlayerStatsChanged -= OnPlayerStatsChanged;
        DataEvents.OnInventoryDataChanged -= ChangeInventoryItems;

        GameEvents.OnFoodStateChanged -= OnFoodChanged;
    }

    private void OnFoodChanged(float amount)
    {
        FloatingText text = Instantiate(_foodChangedText, transform);
        text.Init(amount);
    }

    private void UpdateTimer()
    {
        if (TutorialManager.Instance.IsPaused)
        {
            return;
        }

        _timer.text = TimeUtils.GetFormattedTimeFromSeconds(LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive++);
    }

    private void OnPlayerStatsChanged(PlayerStats playerStats)
    {
        for (int i = 0; i < _foodUnits.Count; i++)
        {
            float fillAmount;
            if (i < playerStats.CurrentFood - 1)
            {
                fillAmount = 1;
            }
            else if (i == playerStats.CurrentFood - 1)
            {
                fillAmount = (float)playerStats.CurrentTimeToEatFood / (float)playerStats.TimeToEatFood;
            }
            else
            {
                fillAmount = 0;
            }

            _foodUnits[i].fillAmount = fillAmount;
        }

        _statusEffects.HandleStatusEffects(playerStats);
    }

    private void ChangeInventoryItems(InventoryData inventoryData)
    {
        if (inventoryData.ItemsInInventory[inventoryData.CurrentHighlightIndex] == null)
        {
            _itemSlots[inventoryData.CurrentHighlightIndex].Occupied = false;
            ChangeHighlight(-1);
        }

        for (int i = 0; i < inventoryData.ItemsInInventory.Count; i++)
        {
            bool enable = inventoryData.ItemsInInventory[i] != null;
            _itemSlots[i].gameObject.SetActive(enable);
            if (enable)
            {
                _itemSlots[i].Init(inventoryData.ItemsInInventory[i]);
            }
            else
            {
                _itemSlots[i].Disable();
            }
        }

        if (_itemSlots.Count > 0)
        {
            if (_currentHighlightIndex != inventoryData.CurrentHighlightIndex)
            {
                RemoveHighlight(_currentHighlightIndex);
            }

            HighlightItem(inventoryData.CurrentHighlightIndex);
            _currentHighlightIndex = inventoryData.CurrentHighlightIndex;
        }
    }

    private void ChangeHighlight(int direction)
    {
        RemoveHighlight(LocalDataStorage.Instance.PlayerData.InventoryData.CurrentHighlightIndex);
        if (_itemSlots.Count == 0)
        {
            return;
        }

        int nextHighlight = LocalDataStorage.Instance.PlayerData.InventoryData.CurrentHighlightIndex;
        for (int i = 0; i < _itemSlots.Count; i++)
        {
            nextHighlight = (nextHighlight + direction + _itemSlots.Count) % _itemSlots.Count;
            if (_itemSlots[nextHighlight].Occupied)
            {
                break;
            }
        }

        if (!_itemSlots[nextHighlight].Occupied)
        {
            return;
        }

        LocalDataStorage.Instance.PlayerData.InventoryData.CurrentHighlightIndex = nextHighlight;
        HighlightItem(LocalDataStorage.Instance.PlayerData.InventoryData.CurrentHighlightIndex);
    }

    private void HighlightItem(int index)
    {
        if (index >= 0 && index < _itemSlots.Count && _itemSlots[index].gameObject.activeSelf)
        {
            _itemSlots[index].Highlight();
        }
    }

    private void RemoveHighlight(int index)
    {
        if (index >= 0 && index < _itemSlots.Count)
        {
            _itemSlots[index].Unhighlight();
        }
    }
}

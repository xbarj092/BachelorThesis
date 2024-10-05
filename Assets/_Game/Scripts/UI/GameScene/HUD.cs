using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private List<Image> _foodUnits;
    [SerializeField] private List<ItemSlot> _itemSlots;

    private void OnEnable()
    {
        DataEvents.OnPlayerStatsChanged += ChangeFoodAmount;
        DataEvents.OnInventoryDataChanged += ChangeInventoryItems;
    }

    private void OnDisable()
    {
        DataEvents.OnPlayerStatsChanged -= ChangeFoodAmount;
        DataEvents.OnInventoryDataChanged -= ChangeInventoryItems;
    }

    private void ChangeFoodAmount(PlayerStats playerStats)
    {
        for (int i = 0; i < _foodUnits.Count; i++)
        {
            _foodUnits[i].gameObject.SetActive(i < playerStats.CurrentFood);
        }
    }

    private void ChangeInventoryItems(InventoryData inventoryData)
    {
        for (int i = 0; i < inventoryData.ItemsInInventory.Count; i++)
        {
            bool enable = inventoryData.ItemsInInventory[i] != null;
            _itemSlots[i].gameObject.SetActive(enable);
            if (enable)
            {
                _itemSlots[i].Init(inventoryData.ItemsInInventory[i]);
            }
        }
    }
}

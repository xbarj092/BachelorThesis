using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.UI;

public class GameSaveInventoryUI : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<ItemType, Sprite> _itemSprites = new();
    [SerializeField] private Image _inventoryIconPrefab;

    public void Init(SavedInventoryData savedInventoryData)
    {
        foreach (SavedItem item in savedInventoryData.SavedItems)
        {
            Image image = Instantiate(_inventoryIconPrefab, transform);
            image.sprite = _itemSprites[(ItemType)item.ItemType];
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;

    public void Init(Item item)
    {
        _itemImage.sprite = item.Stats.Sprite;
    }
}

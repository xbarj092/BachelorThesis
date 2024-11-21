using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image _itemImage;

    [SerializeField] private Image _borderImage;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _highlightedSprite;

    public bool Occupied = false;
    public Item Item;

    public void Init(Item item)
    {
        _itemImage.sprite = item.Stats.Sprite;
        Occupied = true;
    }

    public void Highlight()
    {
        _borderImage.sprite = _highlightedSprite;
    }

    public void Unhighlight()
    {
        _borderImage.sprite = _defaultSprite;
    }
}

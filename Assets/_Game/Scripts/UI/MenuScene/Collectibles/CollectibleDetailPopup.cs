using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleDetailPopup : Popup
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private Image _icon;

    public void Init(ICollectible collectible)
    {
        _title.text = collectible.Title;
        _description.text = collectible.Description;
        _icon.sprite = collectible.Sprite;
    }
}

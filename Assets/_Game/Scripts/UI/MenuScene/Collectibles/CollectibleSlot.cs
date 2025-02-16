using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleSlot : MonoBehaviour
{
    [SerializeField] private Image _sprite;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private ColoredUIHighlighter _highlighter;

    private ICollectible _collectible;
    private bool _hasCollectible;

    public event Action<ICollectible> OnClick;

    public void Init(ICollectible collectible)
    {
        _collectible = collectible;

        _sprite.sprite = _collectible.Sprite;
        _hasCollectible = LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.HasItem(_collectible);
        if (!_hasCollectible)
        {
            _sprite.color = Color.black;
        }
        else
        {
            _title.text = _collectible.Title;
        }
    }

    public void ShowDetail()
    {
        if (_hasCollectible)
        {
            OnClick?.Invoke(_collectible);
        }
    }

    public void Highlight()
    {
        if (_hasCollectible)
        {
            _highlighter.Highlight();
        }
    }

    public void Unhighlight()
    {
        if (_hasCollectible)
        {
            _highlighter.Unhighlight();
        }
    }
}

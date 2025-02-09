using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleSlot : MonoBehaviour
{
    [SerializeField] private Image _sprite;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private GameObject _locked;

    private ICollectible _collectible;

    public event Action<ICollectible> OnHighlight;
    public event Action OnUnhighlight;

    public void Init(ICollectible collectible)
    {
        _collectible = collectible;

        _sprite.sprite = _collectible.Sprite;
        _title.text = _collectible.Title;
    }

    public void Highlight()
    {
        OnHighlight?.Invoke(_collectible);
    }

    public void Unhighlight()
    {
        OnUnhighlight?.Invoke();
    }
}

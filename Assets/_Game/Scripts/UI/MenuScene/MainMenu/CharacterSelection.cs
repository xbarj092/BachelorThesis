using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private TextFieldResizer _textFieldResizer;
    [SerializeField] private ChangeNamePopup _changeNamePopup;
    [SerializeField] private Image _playerIcon;
    [SerializeField] private List<Sprite> _characterSprites = new();

    private ChangeNamePopup _changeNamePopupInstantiated;
    private int _currentIconIndex = 0;

    private readonly List<string> _properties = new()
    {
        "Impaled", "Swift", "Mighty", "Invisible", "Grim", "Wild", "Daring", "Vicious",
        "Enchanted", "Frozen", "Radiant", "Furious", "Stealthy", "Cursed", "Brilliant",
        "Savage", "Burning", "Dark", "Golden", "Silent", "Blazing", "Frosty", "Ancient",
        "Stormy", "Shadowy", "Glorious", "Venomous", "Mystic", "Electric", "Eternal",
        "Vengeful", "Fearless", "Noble", "Spectral", "Infernal", "Divine", "Raging",
        "Arcane", "Boundless", "Hidden"
    };
    private readonly List<string> _things = new()
    {
        "Hat", "Sword", "Shield", "Crown", "Boots", "Ring", "Staff", "Amulet",
        "Blade", "Helm", "Gauntlet", "Mantle", "Orb", "Pendant", "Lantern",
        "Chalice", "Talisman", "Cape", "Gem", "Tome", "Bow", "Arrow", "Spear",
        "Torch", "Wand", "Fang", "Horn", "Scroll", "Banner", "Pendant",
        "Necklace", "Bracelet", "Mask", "Mirror", "Crystal", "Key", "Totem",
        "Dagger", "Grimoire", "Sigil"
    };

    private void Awake()
    {
        StartCoroutine(SetPlayerName());
        _playerIcon.sprite = _characterSprites[LocalDataStorage.Instance.PlayerData.PlayerStats.SpriteIndex];
    }

    private IEnumerator SetPlayerName()
    {
        string name = LocalDataStorage.Instance.PlayerPrefs.LoadPlayerName();
        if (string.IsNullOrEmpty(name))
        {
            yield return StartCoroutine(LootLockerManager.Instance.GetPlayerName((response) =>
            {
                name = response;
            }));

            if (string.IsNullOrEmpty(name))
            {
                name = BuildRandomName();
                LootLockerManager.Instance.SetPlayerName(name);
            }

        }

        _textFieldResizer.UpdateText(name);
    }

    public void ChangeName()
    {
        _changeNamePopupInstantiated = Instantiate(_changeNamePopup, transform.parent);
        _changeNamePopupInstantiated.OnNameChanged += OnNameChanged;
    }

    private void OnNameChanged(string name)
    {
        _textFieldResizer.UpdateText(name);
        LocalDataStorage.Instance.PlayerPrefs.SavePlayerName(name);
        _changeNamePopupInstantiated.OnNameChanged -= OnNameChanged;
    }

    private string BuildRandomName()
    {
        string property = _properties[UnityEngine.Random.Range(0, _properties.Count)];
        string thing = _things[UnityEngine.Random.Range(0, _things.Count)];
        int number = UnityEngine.Random.Range(10, 100);

        string generatedName = $"{property}{thing}{number}";

        return generatedName;
    }

    public void Previous()
    {
        _currentIconIndex--;
        if (_currentIconIndex < 0)
        {
            _currentIconIndex = _characterSprites.Count - 1;
        }

        UpdatePlayerIcon();
    }

    public void Next()
    {
        _currentIconIndex++;
        if (_currentIconIndex >= _characterSprites.Count)
        {
            _currentIconIndex = 0;
        }

        UpdatePlayerIcon();
    }

    private void UpdatePlayerIcon()
    {
        _playerIcon.sprite = _characterSprites[_currentIconIndex];
        LocalDataStorage.Instance.PlayerData.PlayerStats.SpriteIndex = _currentIconIndex;
    }
}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] private TextFieldResizer _nameFieldResizer;
    [SerializeField] private TextFieldResizer _seedFieldResizer;
    [SerializeField] private ChangeNamePopup _changeNamePopup;
    [SerializeField] private Image _playerIcon;
    [SerializeField] private List<MiscCollectibesSO> _collectibles;
    [SerializeField] private TMP_Text _seed;
    [SerializeField] private ChangeSeedPopup _changeSeedPopup;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private Button _leftArrow;

    private ChangeNamePopup _changeNamePopupInstantiated;
    private ChangeSeedPopup _changeSeedPopupInstantiated;

    private List<MiscCollectibesSO> _characterSprites = new();
    private int _currentIconIndex = 0;
    private string _name;

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
        UnlockedCollectibleData unlockedCollectibleData = LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData;
        LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.AddMisc(_collectibles[0]);

        _characterSprites.AddRange(_collectibles.Where(collectible => LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.HasMisc(collectible)));
        _rightArrow.interactable = _characterSprites.Count > 1;
        _leftArrow.interactable = _characterSprites.Count > 1;

        SetPlayerName();
        _seedFieldResizer.UpdateText(Random.Range(999999, int.MaxValue).ToString());
        _playerIcon.sprite = _characterSprites[LocalDataStorage.Instance.PlayerData.PlayerStats.SpriteIndex].Sprite;
    }

    private void SetPlayerName()
    {
        /*yield return StartCoroutine(LootLockerManager.Instance.GetPlayerName((response) =>
        {
            name = response;
        }));*/

        _name = BuildRandomName();

        _nameFieldResizer.UpdateText(_name);
    }

    public void SaveProfile()
    {
        LocalDataStorage.Instance.GameData.GameSeeds = new(int.Parse(_seed.text));
        LocalDataStorage.Instance.PlayerData.PlayerStats.SpriteIndex = _currentIconIndex;
        LootLockerManager.Instance.SetPlayerName(_name);
        LocalDataStorage.Instance.PlayerPrefs.SavePlayerName(_name);
    }

    public void ChangeName()
    {
        _changeNamePopupInstantiated = Instantiate(_changeNamePopup, transform.parent);
        _changeNamePopupInstantiated.OnNameChanged += OnNameChanged;
    }

    private void OnNameChanged(string name)
    {
        _name = name;
        _nameFieldResizer.UpdateText(_name);
        _changeNamePopupInstantiated.OnNameChanged -= OnNameChanged;
    }

    public void ChangeSeed()
    {
        _changeSeedPopupInstantiated = Instantiate(_changeSeedPopup, transform.parent);
        _changeSeedPopupInstantiated.OnSeedChanged += OnSeedChanged;
    }

    private void OnSeedChanged(string seed)
    {
        _seedFieldResizer.UpdateText(seed);
        _changeSeedPopupInstantiated.OnSeedChanged -= OnNameChanged;
    }

    private string BuildRandomName()
    {
        string property = _properties[Random.Range(0, _properties.Count)];
        string thing = _things[Random.Range(0, _things.Count)];
        int number = Random.Range(10, 100);

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
        _playerIcon.sprite = _characterSprites[_currentIconIndex].Sprite;
    }
}

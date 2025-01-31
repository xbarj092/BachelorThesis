using AYellowpaper.SerializedCollections;
using UnityEngine;

public class OptionsScreen : BaseScreen
{
    [SerializeField] private SerializedDictionary<OptionType, OptionPanel> _optionScreens = new();
    [SerializeField] private Transform _spawnTransform;

    private OptionPanel _optionPanelInstantiated;

    public void KeyBindings()
    {
        ShowOptionPanel(OptionType.KeyBindings);
    }

    public void Audio()
    {
        ShowOptionPanel(OptionType.Audio);
    }

    public void Info()
    {
        ShowOptionPanel(OptionType.Info);
    }

    private void ShowOptionPanel(OptionType optionType)
    {
        if (_optionPanelInstantiated != null)
        {
            Destroy(_optionPanelInstantiated.gameObject);
            _optionPanelInstantiated = null;
        }

        _optionPanelInstantiated = Instantiate(_optionScreens[optionType], _spawnTransform);
    }
}

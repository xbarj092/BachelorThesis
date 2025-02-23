using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewGameScreen : BaseScreen
{
    [SerializeField] private CharacterSelection _selection;
    [SerializeField] private Button _newGameButton;

    private void OnEnable()
    {
        _selection.OnIconChanged += OnIconChanged;
    }

    private void OnDisable()
    {
        _selection.OnIconChanged -= OnIconChanged;
    }

    private void OnIconChanged(bool enabled)
    {
        _newGameButton.enabled = enabled;
        _newGameButton.interactable = enabled;
        _newGameButton.GetComponent<EventTrigger>().enabled = enabled;
        _newGameButton.targetGraphic.color = enabled ? Color.white : Color.gray;
    }

    public void StartGame()
    {
        _selection.SaveProfile();
        SceneLoadManager.Instance.GoMenuToGame();
    }
}

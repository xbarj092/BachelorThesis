using UnityEngine;

public class NewGameScreen : BaseScreen
{
    [SerializeField] private CharacterSelection _selection;

    public void StartGame()
    {
        _selection.SaveProfile();
        SceneLoadManager.Instance.GoMenuToGame();
    }
}

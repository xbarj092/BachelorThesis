using UnityEngine;

public class OptionsScreen : BaseScreen
{
    [SerializeField] private GameObject _tutorialButton;

    private void Start()
    {
        _tutorialButton.SetActive(SceneLoadManager.Instance.IsSceneLoaded(SceneLoader.Scenes.MenuScene));
    }

    public void KeyBindings()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.KeyBindings);
    }

    public void AudioSettings()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.AudioSettings);
    }

    public void TutorialSettings()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Tutorials);
    }

    public void AboutGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.AboutGame);
    }
}

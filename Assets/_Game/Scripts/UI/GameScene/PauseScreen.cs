using UnityEngine;

public class PauseScreen : BaseScreen
{
    [SerializeField] private GameObject _tutorialSkipButton;

    private void Start()
    {
        _tutorialSkipButton.SetActive(TutorialManager.Instance.IsTutorialPlaying());
    }

    public void Resume()
    {
        Time.timeScale = 1;
        CloseScreen();
    }

    public void SkipCurrentTutorial()
    {
        TutorialManager.Instance.SkipCurrentTutorial();
        CloseScreen();
    }

    public void Restart()
    {
        LocalDataStorage.Instance.GameData.CurrentSave = null;
        SceneLoadManager.Instance.RestartGame();
    }

    public void Options()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Options);
    }

    public void GoMenu()
    {
        SceneLoadManager.Instance.GoGameToMenu();
    }

    public void Exit()
    {
        Application.Quit();
    }
}

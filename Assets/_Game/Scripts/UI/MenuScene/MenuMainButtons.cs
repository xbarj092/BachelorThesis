using UnityEngine;

public class MenuMainButtons : BaseScreen
{
    public void PlayTheGame()
    {
        SceneLoadManager.Instance.GoMenuToGame();
    }

    public void LoadGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.LoadGame);
    }

    public void Options()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Options);
    }

    public void Leaderboard()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Leaderboard);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

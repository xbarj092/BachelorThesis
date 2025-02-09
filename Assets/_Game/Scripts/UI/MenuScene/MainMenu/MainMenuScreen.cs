using UnityEngine;

public class MainMenuScreen : BaseScreen
{
    public void PlayGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.SelectGameMode);
    }

    public void Options()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Options);
    }

    public void Collectibles()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Collectibles);
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

using UnityEngine;

public class MainMenuScreen : BaseScreen
{
    public void PlayTheGame()
    {
        LocalDataStorage.Instance.GameData.GameSeeds = new(Random.Range(1, int.MaxValue));
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

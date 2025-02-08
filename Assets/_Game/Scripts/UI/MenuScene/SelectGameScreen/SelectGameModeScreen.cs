public class SelectGameModeScreen : BaseScreen
{
    public void ContinueGame()
    {
        // TODO
    }

    public void NewGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.NewGame);
    }

    public void LoadGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.LoadGame);
    }
}

public class OptionsScreen : BaseScreen
{
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

    public void AboutGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.AboutGame);
    }
}

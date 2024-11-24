public class LoadingScreen : GameScreen
{
    private void OnEnable()
    {
        GameEvents.OnMapLoaded += OnMapLoaded;
    }

    private void OnDisable()
    {
        GameEvents.OnMapLoaded -= OnMapLoaded;
    }

    private void OnMapLoaded()
    {
        Close();
    }
}

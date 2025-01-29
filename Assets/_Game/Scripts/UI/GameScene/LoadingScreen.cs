public class LoadingScreen : BaseScreen
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
        Destroy(gameObject);
    }
}

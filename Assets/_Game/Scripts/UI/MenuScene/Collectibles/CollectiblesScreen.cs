public class CollectiblesScreen : BaseScreen
{
    public void Items()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Items);
    }

    public void Enemies()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Enemies);
    }

    public void Food()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Food);
    }

    public void Misc()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Misc);
    }
}

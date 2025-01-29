using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    public GameScreenType GameScreenType;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void CloseScreen()
    {
        ScreenEvents.OnGameScreenClosedInvoke(GameScreenType);
    }
}

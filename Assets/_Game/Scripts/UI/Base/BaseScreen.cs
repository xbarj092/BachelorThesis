using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    public GameScreenType GameScreenType;

    protected virtual void Update()
    {
        if (CanClose() && Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameScreenType == GameScreenType.Pause)
            {
                Time.timeScale = 1.0f;
            }

            CloseScreen();
        }
    }

    protected virtual bool CanClose()
    {
        return IsValidScreen();
    }

    private bool IsValidScreen()
    {
        return GameScreenType != GameScreenType.MenuMain && 
            GameScreenType != GameScreenType.HUD && 
            GameScreenType != GameScreenType.Death;
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void CloseScreen()
    {
        ScreenEvents.OnGameScreenClosedInvoke(GameScreenType);
    }
}

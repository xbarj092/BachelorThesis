using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    public GameScreenType GameScreenType;

    private void Update()
    {
        if (IsValidScreen() && Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameScreenType == GameScreenType.Pause)
            {
                Time.timeScale = 1.0f;
            }

            CloseScreen();
        }
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

using UnityEngine;

public class KeyboardInputHandler
{
    public void HandleInteraction()
    {
        if (ScreenManager.Instance.ActiveGameScreen != null && 
            ScreenManager.Instance.ActiveGameScreen.GameScreenType == GameScreenType.Pause)
        {
            Time.timeScale = 1;
            ScreenEvents.OnGameScreenClosedInvoke(GameScreenType.Pause);
        }
        else if (ScreenManager.Instance.ActiveGameScreen == null || 
            ScreenManager.Instance.ActiveGameScreen?.GameScreenType != GameScreenType.Death)
        {
            Time.timeScale = 0;
            ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Pause);
        }
    }
}

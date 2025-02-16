using System;
using UnityEngine;

public class KeyboardInputHandler
{
    public void HandleInteraction(Action onSuccess)
    {
        if (ScreenManager.Instance.ActiveGameScreen == null || (ScreenManager.Instance.ActiveGameScreen?.GameScreenType != GameScreenType.Death &&
            ScreenManager.Instance.ActiveGameScreen?.GameScreenType != GameScreenType.Loading))
        {
            Time.timeScale = 0;
            onSuccess?.Invoke();
            ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Pause);
        }
    }
}

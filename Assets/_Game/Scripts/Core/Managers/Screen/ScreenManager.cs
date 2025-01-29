using UnityEngine;

public class ScreenManager : MonoSingleton<ScreenManager>
{
    [field: SerializeField] public BaseScreen ActiveGameScreen { get; private set; }

    public void SetActiveGameScreen(BaseScreen screen)
    {
        ActiveGameScreen = screen;
    }
}

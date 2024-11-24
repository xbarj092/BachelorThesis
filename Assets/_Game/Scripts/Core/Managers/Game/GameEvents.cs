using System;

public static class GameEvents
{
    public static event Action OnMapLoaded;
    public static void OnMapLoadedInvoke()
    {
        OnMapLoaded?.Invoke();
    }
}

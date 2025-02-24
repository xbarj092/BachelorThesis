using System;

public static class GameEvents
{
    public static event Action OnMapLoaded;
    public static void OnMapLoadedInvoke()
    {
        OnMapLoaded?.Invoke();
    }

    public static event Action<float> OnFoodStateChanged;
    public static void OnFoodStateChangedInvoke(float foodAmount)
    {
        OnFoodStateChanged?.Invoke(foodAmount);
    }

    public static event Action OnAppleEaten;
    public static void OnAppleEatenInvoke()
    {
        OnAppleEaten?.Invoke();
    }
}

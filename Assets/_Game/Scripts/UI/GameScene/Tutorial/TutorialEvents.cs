using System;

public static class TutorialEvents
{
    // movement tutorial
    public static event Action OnPlayerMoved;
    public static void OnPlayerMovedInvoke()
    {
        OnPlayerMoved?.Invoke();
    }
    
    // item interactions
    public static event Action OnItemPickedUp;
    public static void OnItemPickedUpInvoke()
    {
        OnItemPickedUp?.Invoke();
    }

    public static event Action OnItemPlacedInInventory;
    public static void OnItemPlacedInInventoryInvoke()
    {
        OnItemPlacedInInventory?.Invoke();
    }

    public static event Action OnItemPickedUpFromInventory;
    public static void OnItemPickedUpFromInventoryInvoke()
    {
        OnItemPickedUpFromInventory?.Invoke();
    }

    // end
    public static event Action OnTutorialCompleted;
    public static void OnTutorialCompletedInvoke()
    {
        OnTutorialCompleted?.Invoke();
    }
}

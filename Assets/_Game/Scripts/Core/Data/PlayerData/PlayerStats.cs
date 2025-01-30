using System;

[Serializable]
public class PlayerStats
{
    public int SpriteIndex;

    public int TimeAlive;

    public float TimeToEatFood;
    public float CurrentTimeToEatFood;

    public int CurrentFood;
    public int MaxFood;

    public PlayerStats(int spriteIndex, int timeAlive, float timeToEatFood, float currentTimeToEatFood, int currentFood, int maxFood)
    {
        SpriteIndex = spriteIndex;
        TimeAlive = timeAlive;
        TimeToEatFood = timeToEatFood;
        CurrentTimeToEatFood = currentTimeToEatFood;
        CurrentFood = currentFood;
        MaxFood = maxFood;
    }
}

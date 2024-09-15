using System;

[Serializable]
public class PlayerStats
{
    public int TimeAlive;

    public float TimeToEatFood;
    public float CurrentTimeToEatFood;

    public int CurrentFood;
    public int MaxFood;

    public PlayerStats(int timeAlive, float timeToEatFood, float currentTimeToEatFood, int currentFood, int maxFood)
    {
        TimeAlive = timeAlive;
        TimeToEatFood = timeToEatFood;
        CurrentTimeToEatFood = currentTimeToEatFood;
        CurrentFood = currentFood;
        MaxFood = maxFood;
    }
}

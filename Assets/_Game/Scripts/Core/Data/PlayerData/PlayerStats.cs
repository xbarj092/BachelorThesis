using System;
using System.Collections.Generic;

[Serializable]
public class PlayerStats
{
    public int SpriteIndex;

    public int TimeAlive;

    public float TimeToEatFood;
    public float CurrentTimeToEatFood;
    public int CurrentFood;
    public int MaxFood;

    public List<StatusEffectData> StatusEffects = new();

    public PlayerStats(int spriteIndex, int timeAlive, float timeToEatFood, float currentTimeToEatFood, int currentFood, int maxFood, bool isInvisible)
    {
        SpriteIndex = spriteIndex;
        TimeAlive = timeAlive;

        TimeToEatFood = timeToEatFood;
        CurrentTimeToEatFood = currentTimeToEatFood;
        CurrentFood = currentFood;
        MaxFood = maxFood;
    }

    public PlayerStats(PlayerStats other)
    {
        if (other == null)
        {
            return;
        }

        SpriteIndex = other.SpriteIndex;
        TimeAlive = other.TimeAlive;
        TimeToEatFood = other.TimeToEatFood;
        CurrentTimeToEatFood = other.CurrentTimeToEatFood;
        CurrentFood = other.CurrentFood;
        MaxFood = other.MaxFood;

        StatusEffects = new List<StatusEffectData>();

        if (other.StatusEffects == null)
        {
            return;
        }

        foreach (StatusEffectData effect in other.StatusEffects)
        {
            StatusEffects.Add(new StatusEffectData(effect.Type, effect.OriginalTimeLeft));
        }
    }
}

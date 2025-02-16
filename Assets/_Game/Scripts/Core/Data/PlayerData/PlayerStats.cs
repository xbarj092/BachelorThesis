using System;
using System.Collections.Generic;

[Serializable]
public class PlayerStats
{
    public int SpriteIndex;

    public int TimeAlive;

    public int CurrentTimeLeft;
    public int MaxTimeLeft;

    public List<StatusEffectData> StatusEffects = new();

    public PlayerStats(int spriteIndex, int timeAlive, int currentTimeLeft, int maxTimeLeft, bool isInvisible)
    {
        SpriteIndex = spriteIndex;
        TimeAlive = timeAlive;
        CurrentTimeLeft = currentTimeLeft;
        MaxTimeLeft = maxTimeLeft;
    }
}

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

[Serializable]
public class StatusEffectData
{
    public int Type;
    public int OriginalTimeLeft;
    public int CurrentTimeLeft;

    public StatusEffectData(int type, int originalTimeLeft)
    {
        Type = type;
        OriginalTimeLeft = originalTimeLeft;
        CurrentTimeLeft = OriginalTimeLeft;
    }
}

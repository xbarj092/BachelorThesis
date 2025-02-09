using System;

[Serializable]
public class PlayerStats
{
    public int SpriteIndex;

    public int TimeAlive;

    public int CurrentTimeLeft;
    public int MaxTimeLeft;

    public bool IsInvisible;
    public int OriginalInvisibilityTimeLeft;
    public int InvisibilityTimeLeft;

    public PlayerStats(int spriteIndex, int timeAlive, int currentTimeLeft, int maxTimeLeft, bool isInvisible)
    {
        SpriteIndex = spriteIndex;
        TimeAlive = timeAlive;
        CurrentTimeLeft = currentTimeLeft;
        MaxTimeLeft = maxTimeLeft;
        IsInvisible = isInvisible;
    }
}

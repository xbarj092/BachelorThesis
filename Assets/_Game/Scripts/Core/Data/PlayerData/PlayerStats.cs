using System;

[Serializable]
public class PlayerStats
{
    public int SpriteIndex;

    public int TimeAlive;

    public int CurrentTimeLeft;
    public int MaxTimeLeft;

    public PlayerStats(int spriteIndex, int timeAlive, int currentTimeLeft, int maxTimeLeft)
    {
        SpriteIndex = spriteIndex;
        TimeAlive = timeAlive;
        CurrentTimeLeft = currentTimeLeft;
        MaxTimeLeft = maxTimeLeft;
    }
}

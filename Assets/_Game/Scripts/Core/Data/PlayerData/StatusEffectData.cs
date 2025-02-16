using System;

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

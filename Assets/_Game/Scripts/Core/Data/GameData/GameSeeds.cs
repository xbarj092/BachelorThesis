using System;

[Serializable]
public class GameSeeds
{
    public int MapGenerationSeed;

    public GameSeeds(int mapGenerationSeed)
    {
        MapGenerationSeed = mapGenerationSeed;
    }
}

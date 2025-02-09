using System;

[Serializable]
public class CollectibleData
{
    public string Title;

    public CollectibleData(ICollectible collectible)
    {
        if (collectible == null)
        {
            return;
        }

        Title = collectible.Title;
    }
}

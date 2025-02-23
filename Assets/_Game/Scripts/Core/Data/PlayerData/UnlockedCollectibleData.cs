using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UnlockedCollectibleData
{
    public List<CollectibleData> UnlockedConsumables = new();
    public List<CollectibleData> UnlockedUseableItems = new();
    public List<CollectibleData> UnlockedEnemies = new();
    public List<CollectibleData> UnlockedMisc = new();

    private void AddToList(ICollectible collectible, List<CollectibleData> list)
    {
        if (collectible == null || collectible != null && list.Any(item => item.Title == collectible.Title))
        {
            return;
        }

        list.Add(new(collectible));

        if (UnlockedConsumables.Count == 3 && UnlockedUseableItems.Count == 6 &&
            UnlockedEnemies.Count == 1 && UnlockedMisc.Count == 1)
        {
            UnlockedMisc.Add(new() { Title = "OG Player" });
        }

        LocalDataStorage.Instance.PlayerPrefs.SaveCollectibles(this);
    }

    public void AddConsumable(ICollectible collectible) => AddToList(collectible, UnlockedConsumables);
    public void AddUseable(ICollectible collectible) => AddToList(collectible, UnlockedUseableItems);
    public void AddEnemies(ICollectible collectible) => AddToList(collectible, UnlockedEnemies);
    public void AddMisc(ICollectible collectible) => AddToList(collectible, UnlockedMisc);

    public bool HasConsumable(ICollectible collectible) => UnlockedConsumables.Any(item => item.Title == collectible.Title);
    public bool HasUseable(ICollectible collectible) => UnlockedUseableItems.Any(item => item.Title == collectible.Title);
    public bool HasEnemy(ICollectible collectible) => UnlockedEnemies.Any(item => item.Title == collectible.Title);
    public bool HasMisc(ICollectible collectible) => UnlockedMisc.Any(item => item.Title == collectible.Title);

    public bool HasItem(ICollectible collectible) => HasConsumable(collectible) || HasUseable(collectible) || HasEnemy(collectible) || HasMisc(collectible);
}

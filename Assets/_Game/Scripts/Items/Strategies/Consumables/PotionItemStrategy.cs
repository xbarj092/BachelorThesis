public class PotionItemStrategy : ConsumableItemStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        PlayerStats playerStats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        playerStats.IsInvisible = true;
        playerStats.OriginalInvisibilityTimeLeft = ((PotionItemSO)item.Stats).Duration;
        playerStats.InvisibilityTimeLeft = playerStats.OriginalInvisibilityTimeLeft;
        LocalDataStorage.Instance.PlayerData.PlayerStats = playerStats;
        item.gameObject.SetActive(false);
    }
}

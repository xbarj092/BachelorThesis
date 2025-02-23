using System;

[Serializable]
public class GameSave
{
    public string Name;
    public byte[] Image;

    public TransformData PlayerTransform;
    public PlayerStats PlayerStats;
    public SavedInventoryData SavedInventoryData;

    public GameSeeds GameSeeds;
    public KittenData KittenData;
    public FoodData FoodData;
    public ItemData ItemData;

    public GameSave(byte[] image, TransformData playerTransform, PlayerStats playerStats, SavedInventoryData savedInventoryData, 
        GameSeeds gameSeeds, KittenData kittenData, FoodData foodData, ItemData itemData)
    {
        Name = LocalDataStorage.Instance.PlayerPrefs.LoadPlayerName();
        Image = image;

        PlayerTransform = playerTransform;
        PlayerStats = new(playerStats);
        SavedInventoryData = savedInventoryData;

        GameSeeds = gameSeeds;
        KittenData = kittenData;
        FoodData = foodData;
        ItemData = itemData;
    }
}

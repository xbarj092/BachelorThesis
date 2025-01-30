using System;

[Serializable]
public class GameSave
{
    public string Name;
    public byte[] Image;

    public TransformData PlayerTransform;
    public PlayerStats PlayerStats;
    public SavedInventoryData SavedInventoryData;

    public MapLayout MapLayout;
    public KittenData KittenData;
    public FoodData FoodData;
    public ItemData ItemData;

    public GameSave(byte[] image, TransformData playerTransform, PlayerStats playerStats, SavedInventoryData savedInventoryData, 
        MapLayout mapLayout, KittenData kittenData, FoodData foodData, ItemData itemData)
    {
        Name = LocalDataStorage.Instance.PlayerPrefs.LoadPlayerName();
        Image = image;

        PlayerTransform = playerTransform;
        PlayerStats = playerStats;
        SavedInventoryData = savedInventoryData;

        MapLayout = mapLayout;
        KittenData = kittenData;
        FoodData = foodData;
        ItemData = itemData;
    }
}

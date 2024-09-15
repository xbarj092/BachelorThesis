using UnityEngine;

/// <summary>
/// Contains global constants used throughout the application.
/// </summary>
public static class GlobalConstants
{
    /// <summary>
    /// Contains tag constants used to identify game objects.
    /// </summary>
    public enum Tags
    {
        Room,
        Hallway,
        Shop,
        Enemy,
        Food,
    }

    public enum Layers
    {
        Interact = 8,
    }

    public class SavedDataPaths
    {
        public class SavedPlayerData
        {
            public static string DATA_PATH_PLAYER_LEVELLING = Application.persistentDataPath + "/levelling.gg";
            public static string DATA_PATH_PLAYER_TRANSFORM = Application.persistentDataPath + "/transform.gg";
            public static string DATA_PATH_PLAYER_STATISTICS = Application.persistentDataPath + "/statistics.gg";
            public static string DATA_PATH_PLAYER_EQUIPPED = Application.persistentDataPath + "/equipped.gg";
            public static string DATA_PATH_PLAYER_EFFECTS = Application.persistentDataPath + "/effects.gg";
            public static string DATA_PATH_PLAYER_INVENTORY = Application.persistentDataPath + "/inventory.gg";
        }

        public class SavedGameData
        {
            public static string DATA_PATH_GAME_MAP = Application.persistentDataPath + "/map.gg";
        }
    }
}

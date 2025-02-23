using UnityEngine;

public static class GlobalConstants
{
    public enum Tags
    {
        Room,
        Hallway,
        Shop,
        Enemy,
        Food,
        Item,
        Player,
        InteractableGhost,
        Shadowcaster,
    }

    public enum Layers
    {
        Player = 7,
        Map = 8,
        Kitten = 11,
        Interact = 13,
        KittenInteraction = 18
    }

    public static class SavedDataPaths
    {
#if UNITY_EDITOR
        public static readonly string BASE_PATH = Application.dataPath + "/Data";
#else
        public static readonly string BASE_PATH = Application.persistentDataPath;
#endif
    }
}

using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSpawnChances", menuName = "Items/SpawnChances", order = 0)]
public class ItemSpawnChance : ScriptableObject
{
    public SerializedDictionary<ItemType, float> ItemSpawnChances = new();
}

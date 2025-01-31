using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "SystemSettings", menuName = "Game/SystemSettings", order = 0)]
public class SystemSettings : ScriptableObject
{
    public enum Environment
    {
        Test,
        ProdProcGen,
        ProdHandMade,
    }

    public Environment CurrentEnvironment;

    [Header("LootLocker API Endpoints")]
    public SerializedDictionary<Environment, string> LootLockerLeaderboardIDs = new();

    [Header("Services Environments")]
    public SerializedDictionary<Environment, string> ServiceEnvironmentIDs = new();
}

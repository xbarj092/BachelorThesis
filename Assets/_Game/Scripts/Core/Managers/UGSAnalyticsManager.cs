using System;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class UGSAnalyticsManager : MonoSingleton<UGSAnalyticsManager>
{
    [SerializeField] private SystemSettings _settings;

    private const string ITEM_PICKUP_ID = "ItemPickedUp";
    private const string ITEM_USAGE_ID = "ItemUsed";
    private const string FOOD_PICKUP_ID = "FoodPickedUp";
    private const string FOOD_STOLEN_ID = "FoodStolen";
    private const string PLAYER_DEATH_ID = "PlayerDeath";

    private const string ITEM_ID_PARAMETER = "ItemId";
    private const string TIME_PARAMETER = "Time";
    private const string FOOD_PICKUP_TIME_REFRESHED_PARAMETER = "FoodPickedUpTimeRefreshed";
    private const string NUMBER_OF_KITTENS_ON_MAP_PARAMETER = "NumberOfKittensAlive";
    private const string SEED_PARAMETER = "Seed";

    protected override async void Init()
    {
        try
        {
            InitializationOptions options = new();
            string envId = _settings.ServiceEnvironmentIDs[_settings.CurrentEnvironment];

            if (envId == null)
            {
                throw new Exception($"No service environment ID available for {_settings.CurrentEnvironment}");
            }

            options.SetEnvironmentName(envId);
            await UnityServices.InitializeAsync(options);

            await UnityServices.InitializeAsync();

            Debug.Log($"Service Initialized: {envId}");

            AnalyticsService.Instance.StartDataCollection();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void RecordItemPickedUp(string itemId, int pickUpTime)
    {
        CustomEvent customEvent = new(ITEM_PICKUP_ID)
        {
            { ITEM_ID_PARAMETER, itemId },
            { TIME_PARAMETER, pickUpTime },
            { SEED_PARAMETER, LocalDataStorage.Instance.GameData.GameSeeds.MapGenerationSeed },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void RecordItemUsed(string itemId, int usedTime)
    {
        CustomEvent customEvent = new(ITEM_USAGE_ID)
        {
            { ITEM_ID_PARAMETER, itemId },
            { TIME_PARAMETER, usedTime },
            { SEED_PARAMETER, LocalDataStorage.Instance.GameData.GameSeeds.MapGenerationSeed },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void RecordFoodPickedUp(int pickUpTime, int secondsAdded, string itemId)
    {
        CustomEvent customEvent = new(FOOD_PICKUP_ID)
        {
            { TIME_PARAMETER, pickUpTime },
            { ITEM_ID_PARAMETER, itemId },
            { FOOD_PICKUP_TIME_REFRESHED_PARAMETER, secondsAdded },
            { SEED_PARAMETER, LocalDataStorage.Instance.GameData.GameSeeds.MapGenerationSeed },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void RecordFoodStolen(int stolenTime)
    {
        CustomEvent customEvent = new(FOOD_STOLEN_ID)
        {
            { TIME_PARAMETER, stolenTime },
            { NUMBER_OF_KITTENS_ON_MAP_PARAMETER, KittenManager.Instance.Kittens.Count },
            { SEED_PARAMETER, LocalDataStorage.Instance.GameData.GameSeeds.MapGenerationSeed },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }

    public void RecordPlayerDeath(int timeAlive)
    {
        CustomEvent customEvent = new(PLAYER_DEATH_ID)
        {
            { TIME_PARAMETER, timeAlive },
            { NUMBER_OF_KITTENS_ON_MAP_PARAMETER, KittenManager.Instance.Kittens.Count },
            { SEED_PARAMETER, LocalDataStorage.Instance.GameData.GameSeeds.MapGenerationSeed },
        };

        AnalyticsService.Instance.RecordEvent(customEvent);
    }
}

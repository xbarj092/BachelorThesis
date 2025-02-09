using System.Collections;
using UnityEngine;

public class MenuCanvasController : BaseCanvasController
{
    [SerializeField] private MainMenuScreen _menuMainButtonsPrefab;
    [SerializeField] private LeaderboardScreen _leaderboardScreenPrefab;
    [SerializeField] private LoadingScreen _loadingScreenPrefab;
    [SerializeField] private SelectGameModeScreen _selectGameModeScreen;
    [SerializeField] private NewGameScreen _newGameScreen;
    [SerializeField] private LoadGameScreen _loadGameScreenPrefab;
    [SerializeField] private OptionsScreen _optionsScreenPrefab;
    [SerializeField] private KeyBindingsScreen _keyBindingsScreen;
    [SerializeField] private AudioSettingsScreen _audioSettingsScreen;
    [SerializeField] private AboutGameScreen _aboutGameScreen;
    [SerializeField] private CollectiblesScreen _collectiblesScreen;
    [SerializeField] private ItemsScreen _itemsScreen;
    [SerializeField] private EnemiesScreen _enemiesScreen;
    [SerializeField] private FoodScreen _foodScreen;
    [SerializeField] private MiscScreen _miscScreen;

    private void Awake()
    {
        // here because leaderboard is disabled ATM
        LootLockerManager.Instance.IsInitialized = true;
        StartCoroutine(WaitForLootlocker());
    }

    private IEnumerator WaitForLootlocker()
    {
        ShowGameScreen(GameScreenType.Loading);
        yield return new WaitUntil(() => LootLockerManager.Instance.IsInitialized == true);
        CloseGameScreen(GameScreenType.Loading);
    }

    protected override BaseScreen GetRelevantScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.MenuMain => Instantiate(_menuMainButtonsPrefab, transform),
            GameScreenType.Leaderboard => Instantiate(_leaderboardScreenPrefab, transform),
            GameScreenType.Loading => Instantiate(_loadingScreenPrefab, transform),
            GameScreenType.SelectGameMode => Instantiate(_selectGameModeScreen, transform),
            GameScreenType.NewGame => Instantiate(_newGameScreen, transform),
            GameScreenType.LoadGame => Instantiate(_loadGameScreenPrefab, transform),
            GameScreenType.Options => Instantiate(_optionsScreenPrefab, transform),
            GameScreenType.KeyBindings => Instantiate(_keyBindingsScreen, transform),
            GameScreenType.AudioSettings => Instantiate(_audioSettingsScreen, transform),
            GameScreenType.AboutGame => Instantiate(_aboutGameScreen, transform),
            GameScreenType.Collectibles => Instantiate(_collectiblesScreen, transform),
            GameScreenType.Items => Instantiate(_itemsScreen, transform),
            GameScreenType.Enemies => Instantiate(_enemiesScreen, transform),
            GameScreenType.Food => Instantiate(_foodScreen, transform),
            GameScreenType.Misc => Instantiate(_miscScreen, transform),
            _ => base.GetRelevantScreen(gameScreenType),
        };
    }

    protected override GameScreenType GetActiveGameScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.Options => GameScreenType.MenuMain,
            GameScreenType.Leaderboard => GameScreenType.MenuMain,
            GameScreenType.Loading => GameScreenType.MenuMain,
            GameScreenType.SelectGameMode => GameScreenType.MenuMain,
            GameScreenType.NewGame => GameScreenType.SelectGameMode,
            GameScreenType.LoadGame => GameScreenType.SelectGameMode,
            GameScreenType.KeyBindings => GameScreenType.Options,
            GameScreenType.AudioSettings => GameScreenType.Options,
            GameScreenType.AboutGame => GameScreenType.Options,
            GameScreenType.Collectibles => GameScreenType.MenuMain,
            GameScreenType.Items => GameScreenType.Collectibles,
            GameScreenType.Enemies => GameScreenType.Collectibles,
            GameScreenType.Food => GameScreenType.Collectibles,
            GameScreenType.Misc => GameScreenType.Collectibles,
            _ => base.GetActiveGameScreen(gameScreenType),
        };
    }
}

using System.Collections;
using UnityEngine;

public class MenuCanvasController : BaseCanvasController
{
    [SerializeField] private MainMenuScreen _menuMainButtonsPrefab;
    [SerializeField] private OptionsScreen _optionsScreenPrefab;
    [SerializeField] private LoadGameScreen _loadGameScreenPrefab;
    [SerializeField] private LeaderboardScreen _leaderboardScreenPrefab;
    [SerializeField] private LoadingScreen _loadingScreenPrefab;

    private void Awake()
    {
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
            GameScreenType.Options => Instantiate(_optionsScreenPrefab, transform),
            GameScreenType.LoadGame => Instantiate(_loadGameScreenPrefab, transform),
            GameScreenType.Leaderboard => Instantiate(_leaderboardScreenPrefab, transform),
            GameScreenType.Loading => Instantiate(_loadingScreenPrefab, transform),
            _ => base.GetRelevantScreen(gameScreenType),
        };
    }

    protected override GameScreenType GetActiveGameScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.Options => GameScreenType.MenuMain,
            GameScreenType.LoadGame => GameScreenType.MenuMain,
            GameScreenType.Leaderboard => GameScreenType.MenuMain,
            GameScreenType.Loading => GameScreenType.MenuMain,
            _ => base.GetActiveGameScreen(gameScreenType),
        };
    }
}

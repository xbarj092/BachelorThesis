using UnityEngine;

public class MenuCanvasController : BaseCanvasController
{
    [SerializeField] private MenuMainButtons _menuMainButtonsPrefab;
    [SerializeField] private OptionsScreen _optionsScreenPrefab;
    [SerializeField] private LoadGameScreen _loadGameScreenPrefab;
    [SerializeField] private LeaderboardScreen _leaderboardScreenPrefab;

    private void Awake()
    {
        ShowGameScreen(GameScreenType.MenuMain);
    }

    protected override BaseScreen GetRelevantScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.MenuMain => Instantiate(_menuMainButtonsPrefab, transform),
            GameScreenType.Options => Instantiate(_optionsScreenPrefab, transform),
            GameScreenType.LoadGame => Instantiate(_loadGameScreenPrefab, transform),
            GameScreenType.Leaderboard => Instantiate(_leaderboardScreenPrefab, transform),
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
            _ => base.GetActiveGameScreen(gameScreenType),
        };
    }
}

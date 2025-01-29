using UnityEngine;

public class GameCanvasController : BaseCanvasController
{
    [SerializeField] private DeathScreen _deathScreenPrefab;
    [SerializeField] private PauseScreen _pauseScreenPrefab;

    protected override BaseScreen GetRelevantScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.Death => Instantiate(_deathScreenPrefab, transform),
            GameScreenType.Pause => Instantiate(_pauseScreenPrefab, transform),
            _ => base.GetRelevantScreen(gameScreenType),
        };
    }
}

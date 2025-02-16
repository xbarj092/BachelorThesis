using UnityEngine;

public class GameCanvasController : BaseCanvasController
{
    [SerializeField] private DeathScreen _deathScreenPrefab;
    [SerializeField] private PauseScreen _pauseScreenPrefab;
    [SerializeField] private OptionsScreen _optionsScreenPrefab;
    [SerializeField] private KeyBindingsScreen _keyBindingsScreen;
    [SerializeField] private AudioSettingsScreen _audioSettingsScreen;
    [SerializeField] private TutorialsScreen _tutorialScreen;
    [SerializeField] private ReplayTutorialScreen _replayTutorialScreen;
    [SerializeField] private AboutGameScreen _aboutGameScreen;

    protected override BaseScreen GetRelevantScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.Death => Instantiate(_deathScreenPrefab, transform),
            GameScreenType.Pause => Instantiate(_pauseScreenPrefab, transform),
            GameScreenType.Options => Instantiate(_optionsScreenPrefab, transform),
            GameScreenType.KeyBindings => Instantiate(_keyBindingsScreen, transform),
            GameScreenType.AudioSettings => Instantiate(_audioSettingsScreen, transform),
            GameScreenType.Tutorials => Instantiate(_tutorialScreen, transform),
            GameScreenType.ReplayTutorial => Instantiate(_replayTutorialScreen, transform),
            GameScreenType.AboutGame => Instantiate(_aboutGameScreen, transform),
            _ => base.GetRelevantScreen(gameScreenType),
        };
    }

    protected override GameScreenType GetActiveGameScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.Options => GameScreenType.Pause,
            GameScreenType.KeyBindings => GameScreenType.Options,
            GameScreenType.AudioSettings => GameScreenType.Options,
            GameScreenType.Tutorials => GameScreenType.Options,
            GameScreenType.ReplayTutorial => GameScreenType.Tutorials,
            GameScreenType.AboutGame => GameScreenType.Options,
            _ => base.GetActiveGameScreen(gameScreenType),
        };
    }
}

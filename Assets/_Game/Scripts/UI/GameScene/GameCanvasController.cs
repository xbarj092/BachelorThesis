using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

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
    [SerializeField] private GameWonScreen _gameWonScreen;
    [SerializeField] private Image _whiteScreen;

    [Space(5)]
    [SerializeField] private AudioClip Clip;

    private List<Light2D> _lights;
    private CinemachineVirtualCamera _playerCam;
    private CinemachineBasicMultiChannelPerlin _perlin;

    private void Start()
    {
        _lights = FindObjectsOfType<Light2D>().ToList();
        _playerCam = FindFirstObjectByType<CinemachineVirtualCamera>();
        _perlin = _playerCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    protected override void OnEnable()
    {
        GameEvents.OnAppleEaten += OnAppleEaten;
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        GameEvents.OnAppleEaten -= OnAppleEaten;
        base.OnDisable();
    }

    private void OnAppleEaten()
    {
        StartCoroutine(ShowWin());
    }

    private IEnumerator ShowWin()
    {
        TutorialManager.Instance.IsPaused = true;
        DOTween.To(() => _perlin.m_AmplitudeGain, x => _perlin.m_AmplitudeGain = x, 0.5f, 2f);
        AudioManager.Instance.Play(SoundType.Rumbling);
        foreach (AudioSource source in AudioManager.Instance.gameObject.GetComponents<AudioSource>())
        {
            if (source.clip == Clip)
            {
                DOTween.To(() => source.volume, x => source.volume = x, 1f, 2f);
            }
        }
        
        yield return new WaitForSeconds(2f);
        foreach (Light2D light in _lights)
        {
            DOTween.To(() => light.intensity, x => light.intensity = x, 100f, 2f);
        }
        yield return new WaitForSeconds(2f);
        DOTween.To(() => _whiteScreen.color.a,
           x => {
               Color color = _whiteScreen.color;
               color.a = x;
               _whiteScreen.color = color;
           }, 1f, 2f);
        yield return new WaitForSeconds(2f);
        TutorialManager.Instance.IsPaused = false;
        ShowGameScreen(GameScreenType.Won);
    }

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
            GameScreenType.Won => Instantiate(_gameWonScreen, transform),
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

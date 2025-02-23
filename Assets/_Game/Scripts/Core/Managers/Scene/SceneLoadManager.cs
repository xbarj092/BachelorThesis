using System.Collections;
using UnityEngine;

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    private void OnEnable()
    {
        DataEvents.OnDataLoaded += OnDataLoaded;
    }

    private void OnDisable()
    {
        DataEvents.OnDataLoaded -= OnDataLoaded;
    }

    private void OnDataLoaded()
    {
        if (IsSceneLoaded(SceneLoader.Scenes.MenuScene))
        {
            GoMenuToGame();
        }
        else if (IsSceneLoaded(SceneLoader.Scenes.GameScene))
        {
            RestartGame();
        }
    }

    protected override void Init()
    {
        base.Init();
        GoBootToMenu();
    }

    public void GoBootToMenu()
    {
        SceneLoader.OnSceneLoadDone += OnBootToMenuLoadDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.MenuScene);
    }

    private void OnBootToMenuLoadDone(SceneLoader.Scenes scene)
    {
        PlayMenuMusic();
        Time.timeScale = 1;
        SceneLoader.OnSceneLoadDone -= OnBootToMenuLoadDone;
    }

    public void GoMenuToGame()
    {
        SceneLoader.OnSceneLoadDone += OnMenuToGameLoadDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.GameScene, toUnload: SceneLoader.Scenes.MenuScene);
    }

    private void OnMenuToGameLoadDone(SceneLoader.Scenes scenes)
    {
        StartCoroutine(InitGame());
        SceneLoader.OnSceneLoadDone -= OnMenuToGameLoadDone;
    }

    public void GoGameToMenu()
    {
        SceneLoader.OnSceneLoadDone += OnGameToMenuLoadDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.MenuScene, toUnload: SceneLoader.Scenes.GameScene);
    }

    private void OnGameToMenuLoadDone(SceneLoader.Scenes scenes)
    {
        Time.timeScale = 1;
        LocalDataStorage.Instance.GameData.CurrentSave = null;
        PlayMenuMusic();
        SceneLoader.OnSceneLoadDone -= OnGameToMenuLoadDone;
    }

    public void RestartGame()
    {
        SceneLoader.OnSceneLoadDone += OnRestartGameDone;
        SceneLoader.LoadScene(SceneLoader.Scenes.GameScene, toUnload: SceneLoader.Scenes.GameScene);
    }

    private void OnRestartGameDone(SceneLoader.Scenes scenes)
    {
        LocalDataStorage.Instance.GameData.Random = new(LocalDataStorage.Instance.GameData.GameSeeds.MapGenerationSeed);
        StartCoroutine(InitGame());
        SceneLoader.OnSceneLoadDone -= OnRestartGameDone;
    }

    public bool IsSceneLoaded(SceneLoader.Scenes sceneToCheck)
    {
        return SceneLoader.IsSceneLoaded(sceneToCheck);
    }

    private IEnumerator InitGame()
    {
        ScreenManager.Instance.ActiveGameScreen = FindFirstObjectByType<LoadingScreen>();
        GameManager.Instance.MapInitialized = false;
        Time.timeScale = 1;
        MapGenerator.MapGenerator generator = FindObjectOfType<MapGenerator.MapGenerator>();
        KittenManager.Instance.ResetManager();
        KittenManager.Instance.SpawnTransform = generator.KittenSpawnTransform;
        KittenManager.Instance.AStar = generator.AStar;
        ItemManager.Instance.ResetManager();
        GameSave currentSave = LocalDataStorage.Instance.GameData.CurrentSave;
        bool loaded = currentSave != null && !string.IsNullOrEmpty(currentSave.Name);
        generator.LoadedData = loaded;
        yield return StartCoroutine(generator.GenerateMap());

        SpawnEntities();
        LocalDataStorage.Instance.InitPlayerData(loaded);

        PlayGameMusic();
        yield return new WaitForSeconds(1);
        GameManager.Instance.MapInitialized = true;
        GameEvents.OnMapLoadedInvoke();
        KittenManager.Instance.Initialize();

        if (!TutorialManager.Instance.IsTutorialCompleted(TutorialID.GeneralInfo))
        {
            TutorialManager.Instance.InstantiateTutorial(TutorialID.GeneralInfo);
        }
        else if (!TutorialManager.Instance.IsTutorialCompleted(TutorialID.Movement))
        {
            TutorialManager.Instance.InstantiateTutorial(TutorialID.Movement);
        }
    }

    private void SpawnEntities()
    {
        Player player = FindObjectOfType<Player>();

        GameSave currentSave = LocalDataStorage.Instance.GameData.CurrentSave;
        if (currentSave != null && !string.IsNullOrEmpty(currentSave.Name))
        {
            LocalDataStorage.Instance.PlayerData.PlayerTransform.ApplyToTransform(player.transform);
            // kittens
        }
        else
        {
            if (player != null)
            {
                player.transform.position = GameManager.Instance.StartRoomLocation;
            }

            KittenManager.Instance.CreateKitten(new(50, 50, 0), false);
        }
    }

    private void PlayMenuMusic()
    {
        if (AudioManager.Instance.IsPlaying(SoundType.GameAmbience))
        {
            AudioManager.Instance.Stop(SoundType.GameAmbience);
        }
    }

    private void PlayGameMusic()
    {
        if (!AudioManager.Instance.IsPlaying(SoundType.GameAmbience))
        {
            AudioManager.Instance.Play(SoundType.GameAmbience);
        }
    }
}

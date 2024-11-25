using System;
using System.Collections;
using UnityEngine;

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
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
        StartCoroutine(InitGame());
        SceneLoader.OnSceneLoadDone -= OnRestartGameDone;
    }

    public bool IsSceneLoaded(SceneLoader.Scenes sceneToCheck)
    {
        return SceneLoader.IsSceneLoaded(sceneToCheck);
    }

    private IEnumerator InitGame()
    {
        GameManager.Instance.MapInitialized = false;
        Time.timeScale = 1;
        MapGenerator.MapGenerator generator = FindObjectOfType<MapGenerator.MapGenerator>();
        KittenManager.Instance.SpawnTransform = generator.KittenSpawnTransform;
        KittenManager.Instance.AStar = generator.AStar;
        yield return StartCoroutine(generator.GenerateMap());
        SpawnEntities();
        LocalDataStorage.Instance.InitPlayerData();
        PlayAmbience();
        yield return new WaitForSeconds(1);
        GameManager.Instance.MapInitialized = true;
        GameEvents.OnMapLoadedInvoke();
    }

    private void SpawnEntities()
    {
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.transform.position = new(24, 24);
        }

        KittenManager.Instance.CreateKitten(new(26, 26, 0), true);
        KittenManager.Instance.CreateKitten(new(50, 50, 0), false);
    }

    private void PlayMenuMusic()
    {
        /*if (AudioManager.Instance.IsPlaying(SoundType.GameAmbience))
        {
            AudioManager.Instance.Stop(SoundType.GameAmbience);
        }
        
        if (!AudioManager.Instance.IsPlaying(SoundType.MenuAmbience))
        {
            AudioManager.Instance.Play(SoundType.MenuAmbience);
        }*/
    }

    private void PlayAmbience()
    {
        /*if (AudioManager.Instance.IsPlaying(SoundType.MenuAmbience))
        {
            AudioManager.Instance.Stop(SoundType.MenuAmbience);
        }

        if (!AudioManager.Instance.IsPlaying(SoundType.GameAmbience))
        {
            AudioManager.Instance.Play(SoundType.GameAmbience);
        }*/
    }
}

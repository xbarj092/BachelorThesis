using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsScreen : BaseScreen
{
    [SerializeField] private GameObject _tutorialButton;
    [SerializeField] private GameObject _saveButton;
    [SerializeField] private GameObject _loadButton;

    [Space(5)]
    [SerializeField] private GameObject _savingVisual;
    [SerializeField] private List<Button> _buttons = new();

    public bool IsSaving = false;

    private void Start()
    {
        GameSave currentSave = LocalDataStorage.Instance.GameData.CurrentSave;

        _tutorialButton.SetActive(SceneLoadManager.Instance.IsSceneLoaded(SceneLoader.Scenes.MenuScene));
        _saveButton.SetActive(SceneLoadManager.Instance.IsSceneLoaded(SceneLoader.Scenes.GameScene));
        _loadButton.SetActive(SceneLoadManager.Instance.IsSceneLoaded(SceneLoader.Scenes.GameScene) && currentSave != null && !string.IsNullOrEmpty(currentSave.Name));
    }

    public void KeyBindings()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.KeyBindings);
    }

    public void AudioSettings()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.AudioSettings);
    }

    public void TutorialSettings()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Tutorials);
    }

    public void LoadLastSave()
    {
        GameSave currentSave = LocalDataStorage.Instance.GameData.CurrentSave;
        if (currentSave == null && string.IsNullOrEmpty(currentSave.Name))
        {
            SceneLoadManager.Instance.RestartGame();
        }
        else
        {
            SetSaveState(true);
            StartCoroutine(LocalDataStorage.Instance.LoadData(currentSave));
        }
    }

    public void SaveGame()
    {
        SetSaveState(true);
        StartCoroutine(LocalDataStorage.Instance.SaveData(CaptureScreenshot(Screen.width, Screen.height).EncodeToJPG(), () => { SetSaveState(false); }));
    }

    public void SetSaveState(bool isSaving)
    {
        _savingVisual.SetActive(isSaving);
        IsSaving = isSaving;
        foreach (Button button in _buttons)
        {
            button.interactable = !isSaving;
        }
    }

    public Texture2D CaptureScreenshot(int width, int height)
    {
        GameObject screenshotCamObject = new("ScreenshotCamera");
        Camera screenshotCam = screenshotCamObject.AddComponent<Camera>();

        Camera mainCam = Camera.main;
        screenshotCam.CopyFrom(mainCam);

        screenshotCam.cullingMask = ~LayerMask.GetMask("UI");

        RenderTexture renderTexture = new(width, height, 24);
        screenshotCam.targetTexture = renderTexture;
        screenshotCam.Render();

        Texture2D screenshot = new(width, height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        RenderTexture.active = null;
        screenshotCam.targetTexture = null;
        renderTexture.Release();
        Destroy(screenshotCamObject);

        return screenshot;
    }

    public void AboutGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.AboutGame);
    }
}

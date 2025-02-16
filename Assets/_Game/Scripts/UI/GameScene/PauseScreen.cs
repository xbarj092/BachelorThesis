using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseScreen : BaseScreen
{
    [SerializeField] private GameObject _savingVisual;
    [SerializeField] private List<Button> _buttons = new();
    [SerializeField] private GameObject _tutorialSkipButton;

    public bool IsSaving = false;

    private void Start()
    {
        _tutorialSkipButton.SetActive(TutorialManager.Instance.IsTutorialPlaying());
    }

    public void Resume()
    {
        Time.timeScale = 1;
        CloseScreen();
    }

    public void SkipCurrentTutorial()
    {
        TutorialManager.Instance.SkipCurrentTutorial();
        CloseScreen();
    }

    public void Restart()
    {
        SceneLoadManager.Instance.RestartGame();
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

    public void Options()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Options);
    }

    public void GoMenu()
    {
        SceneLoadManager.Instance.GoGameToMenu();
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ToggleSound()
    {
        // TODO - sounds
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
}

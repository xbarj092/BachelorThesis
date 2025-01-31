using UnityEngine;

public class PauseScreen : BaseScreen
{
    public void Resume()
    {
        Time.timeScale = 1;
        CloseScreen();
    }

    public void Restart()
    {
        SceneLoadManager.Instance.RestartGame();
    }

    public void SaveGame()
    {
        StartCoroutine(LocalDataStorage.Instance.SaveData(CaptureScreenshot(Screen.width, Screen.height).EncodeToJPG()));
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

using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SelectGameModeScreen : BaseScreen
{
    public void ContinueGame()
    {
        List<string> saveFiles = new();
        string path = GlobalConstants.SavedDataPaths.BASE_PATH;
        if (Directory.Exists(path))
        {
            saveFiles = Directory.GetFiles(path, "*.gg").Select(Path.GetFileName).ToList();
        }

        GameSave gameSave = LocalDataStorage.Instance.GetSaveFileFromPath(saveFiles.Last());
        StartCoroutine(LocalDataStorage.Instance.LoadData(gameSave));
    }

    public void NewGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.NewGame);
    }

    public void LoadGame()
    {
        Destroy(gameObject);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.LoadGame);
    }
}

using TMPro;
using UnityEngine;

public class DeathScreen : BaseScreen
{
    [SerializeField] private TMP_Text _survivalTimeText;

    private const string SURVIVAL_TIME_TEXT_PREFIX = "You survived for ";

    private void Start()
    {
        Time.timeScale = 0;
        _survivalTimeText.text = SURVIVAL_TIME_TEXT_PREFIX + TimeUtils.GetFormattedTimeFromSeconds(LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
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
            StartCoroutine(LocalDataStorage.Instance.LoadData(currentSave));
        }
    }

    public void RestartGame()
    {
        LocalDataStorage.Instance.GameData.CurrentSave = null;
        SceneLoadManager.Instance.RestartGame();
    }

    public void GoToMenu()
    {
        SceneLoadManager.Instance.GoGameToMenu();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

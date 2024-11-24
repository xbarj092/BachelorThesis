using TMPro;
using UnityEngine;

public class DeathScreen : GameScreen
{
    [SerializeField] private TMP_Text _survivalTimeText;

    private const string SURVIVAL_TIME_TEXT_PREFIX = "You survived for ";

    private void Start()
    {
        _survivalTimeText.text = SURVIVAL_TIME_TEXT_PREFIX + TimeUtils.GetFormattedTimeFromSeconds(LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
    }

    public void PlayAgain()
    {
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

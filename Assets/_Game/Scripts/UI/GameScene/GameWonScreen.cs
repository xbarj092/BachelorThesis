using TMPro;
using UnityEngine;

public class GameWonScreen : BaseScreen
{
    [SerializeField] private TMP_Text _survivalTimeText;

    private const string SURVIVAL_TIME_TEXT_PREFIX = "You survived for ";

    private void Start()
    {
        Time.timeScale = 0;
        _survivalTimeText.text = SURVIVAL_TIME_TEXT_PREFIX + TimeUtils.GetFormattedTimeFromSeconds(LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
    }

    public void Continue()
    {
        SceneLoadManager.Instance.GoGameToMenu();
    }

    public void Exit()
    {
        Application.Quit();
    }
}

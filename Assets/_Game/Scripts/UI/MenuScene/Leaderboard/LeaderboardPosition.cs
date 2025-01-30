using TMPro;
using UnityEngine;

public class LeaderboardPosition : MonoBehaviour
{
    [SerializeField] private TMP_Text _playerPosition;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _playerScore;

    public void Init(string position, string name, int score)
    {
        _playerPosition.text = position;
        _playerName.text = name;
        _playerScore.text = TimeUtils.GetFormattedTimeFromSeconds(score);
    }
}

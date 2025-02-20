using LootLocker.Requests;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardScreen : BaseScreen
{
    [SerializeField] private LeaderboardPosition _leaderboardPosition;
    [SerializeField] private RectTransform _spawnTransform;
    [SerializeField] private TMP_Text _noPlayersText;

    private const string LOADING_TEXT = "LOADING...";
    private const string NO_PLAYERS_FOUND_TEXT = "NO PLAYERS FOUND";

    private void Awake()
    {
        _noPlayersText.text = LOADING_TEXT;
        InvokeRepeating(nameof(Refresh), 0, 10);
    }

    private void Refresh()
    {
        if (TutorialManager.Instance.IsPaused)
        {
            return;
        }

        StartCoroutine(LootLockerManager.Instance.FetchTopHighscoresRoutine(SetUpLeaderboardPositions));
    }

    private void SetUpLeaderboardPositions(List<LootLockerLeaderboardMember> members)
    {
        _noPlayersText.text = NO_PLAYERS_FOUND_TEXT;
        _noPlayersText.gameObject.SetActive(members.Count == 0);

        int i = 0;
        foreach (LootLockerLeaderboardMember member in members)
        {
            i++;
            LeaderboardPosition position = Instantiate(_leaderboardPosition, _spawnTransform);
            position.Init(i.ToString(), member.player.name, member.score);
        }
    }
}

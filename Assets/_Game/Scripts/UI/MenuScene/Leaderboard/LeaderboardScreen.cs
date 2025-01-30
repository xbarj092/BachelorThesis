using LootLocker.Requests;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardScreen : BaseScreen
{
    [SerializeField] private LeaderboardPosition _leaderboardPosition;
    [SerializeField] private RectTransform _spawnTransform;

    private void Awake()
    {
        StartCoroutine(LootLockerManager.Instance.FetchTopHighscoresRoutine(SetUpLeaderboardPositions));
    }

    private void SetUpLeaderboardPositions(List<LootLockerLeaderboardMember> members)
    {
        int i = 0;
        foreach (LootLockerLeaderboardMember member in members)
        {
            i++;
            LeaderboardPosition position = Instantiate(_leaderboardPosition, _spawnTransform);
            position.Init(i.ToString(), member.member_id, member.score);
        }
    }
}

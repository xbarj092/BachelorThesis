using LootLocker.Requests;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootLockerManager : MonoSingleton<LootLockerManager>
{
    [SerializeField] private SystemSettings _settings;

    private void Start()
    {
        StartCoroutine(LoginRoutine());
    }

    public string GetLeaderboardID()
    {
        return _settings.LootLockerLeaderboardIDs[_settings.CurrentEnvironment];
    }

    private IEnumerator LoginRoutine()
    {
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Player was logged in");
                LocalDataStorage.Instance.PlayerPrefs.SavePlayerId(response.player_id.ToString());
                done = true;
            }
            else
            {
                Debug.Log("Could not start session");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator SubmitScore(int scoreToUpload)
    {
        bool done = false;
        string playerId = LocalDataStorage.Instance.PlayerPrefs.LoadPlayerId();
        LootLockerSDKManager.SubmitScore(playerId, scoreToUpload, GetLeaderboardID(), (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully uploaded score");
                done = true;
            }
            else
            {
                Debug.Log("Failed - " + response.errorData);
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
    }

    public IEnumerator FetchTopHighscoresRoutine(System.Action<List<LootLockerLeaderboardMember>> callback)
    {
        bool done = false;
        List<LootLockerLeaderboardMember> leaderboardMembers = new();

        LootLockerSDKManager.GetScoreList(GetLeaderboardID(), 10, 0, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully fetched leaderboard data");
                leaderboardMembers = response.items.ToList();
            }
            else
            {
                Debug.Log("Failed to fetch leaderboard scores");
            }
            done = true;
        });

        yield return new WaitWhile(() => !done);
        callback?.Invoke(leaderboardMembers);
    }

    public void SetPlayerName(string playerName)
    {
        LootLockerSDKManager.SetPlayerName(playerName, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully set player name");
            }
            else
            {
                Debug.Log("Could not set player name - " + response.errorData);
            }
        });
    }
}

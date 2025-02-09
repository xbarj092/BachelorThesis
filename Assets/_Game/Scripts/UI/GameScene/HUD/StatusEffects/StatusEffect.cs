using UnityEngine;
using UnityEngine.UI;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] private Image _effectImage;
    [SerializeField] private Image _fillImage;

    public StatusEffectType Type;

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void Init(Sprite sprite, StatusEffectType type, int timeLeft)
    {
        Type = type;

        _effectImage.sprite = sprite;
        _fillImage.sprite = sprite;

        InvokeRepeating(nameof(TimeOutInvincibility), 1, 1);
    }

    private void TimeOutInvincibility()
    {
        PlayerStats playerStats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        playerStats.InvisibilityTimeLeft--;
        _fillImage.fillAmount = (float)playerStats.InvisibilityTimeLeft / (float)playerStats.OriginalInvisibilityTimeLeft;

        if (playerStats.InvisibilityTimeLeft <= 0)
        {
            playerStats.IsInvisible = false;
            Destroy(gameObject);
        }

        LocalDataStorage.Instance.PlayerData.PlayerStats = playerStats;
    }
}

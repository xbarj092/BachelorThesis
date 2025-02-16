using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffect : MonoBehaviour
{
    [SerializeField] private Image _effectImage;
    [SerializeField] private Image _fillImage;

    public StatusEffectData EffectData;

    public event Action<StatusEffect> OnStatusEnded; 

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void Init(Sprite sprite, StatusEffectData effectData)
    {
        EffectData = effectData;

        _effectImage.sprite = sprite;
        _fillImage.sprite = sprite;

        InvokeRepeating(nameof(TimeOutEffect), 1, 1);
    }

    private void TimeOutEffect()
    {
        PlayerStats playerStats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        StatusEffectData effectData = playerStats.StatusEffects.FirstOrDefault(effect => effect.Type == EffectData.Type);

        if (effectData != null)
        {
            effectData.CurrentTimeLeft--;
            _fillImage.fillAmount = 1 - ((float)effectData.CurrentTimeLeft / (float)effectData.OriginalTimeLeft);

            if (effectData.CurrentTimeLeft <= 0)
            {
                playerStats.StatusEffects.Remove(effectData);
                OnStatusEnded?.Invoke(this);
                Destroy(gameObject);
            }

            int index = playerStats.StatusEffects.FindIndex(effect => effect.Type == EffectData.Type);
            if (index >= 0)
            {
                playerStats.StatusEffects[index] = effectData;
            }

            LocalDataStorage.Instance.PlayerData.PlayerStats = playerStats;
        }
    }
}

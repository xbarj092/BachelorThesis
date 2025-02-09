using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<StatusEffectType, Sprite> _sprites = new();
    [SerializeField] private StatusEffect _statusEffectPrefab;

    private List<StatusEffect> _activeStatusEffects = new();

    public void HandleStatusEffects(PlayerStats playerStats)
    {
        foreach (StatusEffectData effectData in playerStats.StatusEffects)
        {
            ShowStatusEffect(effectData);
        }
    }

    private void ShowStatusEffect(StatusEffectData effectData)
    {
        if (!_activeStatusEffects.Any(effect => effect.EffectData.Type == effectData.Type))
        {
            StatusEffect effect = Instantiate(_statusEffectPrefab, transform);
            effect.Init(_sprites[(StatusEffectType)effectData.Type], effectData);
            _activeStatusEffects.Add(effect);
        }
    }
}

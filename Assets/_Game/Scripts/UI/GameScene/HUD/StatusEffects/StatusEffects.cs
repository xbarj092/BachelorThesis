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
        if (playerStats.IsInvisible)
        {
            ShowStatusEffect(StatusEffectType.Invisibility, playerStats.InvisibilityTimeLeft);
        }
    }

    private void ShowStatusEffect(StatusEffectType invisibility, int invisibilityTimeLeft)
    {
        if (!_activeStatusEffects.Any(effect => effect.Type == invisibility))
        {
            StatusEffect effect = Instantiate(_statusEffectPrefab, transform);
            effect.Init(_sprites[invisibility], invisibility, invisibilityTimeLeft);
            _activeStatusEffects.Add(effect);
        }
    }
}

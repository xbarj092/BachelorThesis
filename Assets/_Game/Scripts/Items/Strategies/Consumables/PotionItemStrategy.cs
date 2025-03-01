using System.Linq;

public class PotionItemStrategy : ConsumableItemStrategy
{
    public override void PickUp(ConsumableItem item)
    {
        PlayerStats playerStats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        if (!playerStats.StatusEffects.Any(effect => effect.Type == (int)StatusEffectType.Invisibility))
        {
            playerStats.StatusEffects.Add(new((int)StatusEffectType.Invisibility, ((PotionItemSO)item.Stats).Duration));
        }
        else
        {
            playerStats.StatusEffects.FirstOrDefault(effect => effect.Type == (int)StatusEffectType.Invisibility).CurrentTimeLeft = ((PotionItemSO)item.Stats).Duration;
        }

        if (!TutorialManager.Instance.IsTutorialCompleted(TutorialID.StatusEffect) && !TutorialManager.Instance.IsTutorialPlaying())
        {
            TutorialManager.Instance.InstantiateTutorial(TutorialID.StatusEffect);
        }

        LocalDataStorage.Instance.PlayerData.PlayerStats = playerStats;
        base.PickUp(item);
        AudioManager.Instance.Stop(SoundType.FoodPickedUp);
        AudioManager.Instance.Play(SoundType.PotionPickUp);
    }
}

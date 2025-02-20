public class TutorialPotionUseAction : TutorialItemUseAction
{
    public override void Exit()
    {
        if (!TutorialManager.Instance.IsTutorialCompleted(TutorialID.StatusEffect))
        {
            TutorialManager.Instance.InstantiateTutorial(TutorialID.StatusEffect);
        }
    }
}

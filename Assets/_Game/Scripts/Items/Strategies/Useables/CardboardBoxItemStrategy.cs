using UnityEngine;

public class CardboardBoxItemStrategy : UseableItemStrategy
{
    public override bool CanUse(UseableItem item)
    {
        if (TutorialManager.Instance.IsTutorialPlaying(TutorialID.Kittens))
        {
            float distance = Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint((GameObject.FindFirstObjectByType<Player>().transform.position + TutorialManager.Instance.CurrentKittenInRange.transform.position) / 2));

            if (distance > 400)
            {
                return false;
            }
        }

        return true;
    }

    public override void Use(UseableItem item)
    {
        AudioManager.Instance.Play(SoundType.ItemUsedCardboardBox);

        Debug.Log("[CardboardBoxItemStrategy] - Used cardboard box!");
        PlaceOnMousePosition(item);
    }

    public override void PickUp(UseableItem item)
    {
        base.PickUp(item);
        Debug.Log("[CardboardBoxItemStrategy] - Picked up cardboard box!");
    }
}

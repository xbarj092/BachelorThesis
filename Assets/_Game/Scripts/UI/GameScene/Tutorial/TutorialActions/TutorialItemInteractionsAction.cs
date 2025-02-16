using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialItemInteractionsAction : TutorialAction
{
    [SerializeField] private Image _background;

    [SerializeField] private Image _itemOnGroundCutout;
    [SerializeField] private Image _inventoryCutout;
    [SerializeField] private Image _itemCutout;

    private void OnDisable()
    {
        TutorialManager.Instance.IsPaused = false;
        TutorialManager.Instance.CanUseItem = true;
        TutorialEvents.OnItemPickedUp -= OnAfterItemPickedUp;
        TutorialEvents.OnItemPlacedInInventory -= OnAfterItemPlacedInInventory;
        TutorialEvents.OnItemPickedUpFromInventory -= OnAfterItemPickedUpFromInventory;
        TutorialEvents.OnItemDropped -= OnAfterItemDrop;
    }

    public override void StartAction()
    {
        TutorialManager.Instance.IsPaused = true;
        _background.gameObject.SetActive(true);
        StartCoroutine(DelayedItemHighlight());
        TutorialManager.Instance.CurrentItemInRange.Highlight();
        _itemOnGroundCutout.sprite = TutorialManager.Instance.CurrentItemInRange.Stats.Sprite;
        _tutorialPlayer.MoveToNextNarratorText();
        TutorialEvents.OnItemPickedUp += OnAfterItemPickedUp;
    }

    private IEnumerator DelayedItemHighlight()
    {
        yield return new WaitForSeconds(0.5f);
        _itemOnGroundCutout.transform.position = Camera.main.WorldToScreenPoint(TutorialManager.Instance.CurrentItemInRange.transform.position);
        _itemOnGroundCutout.gameObject.SetActive(true);
    }

    private void OnAfterItemPickedUp()
    {
        TutorialEvents.OnItemPickedUp -= OnAfterItemPickedUp;
        _itemOnGroundCutout.gameObject.SetActive(false);
        _inventoryCutout.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        TutorialManager.Instance.CanUseItem = false;
        TutorialEvents.OnItemPlacedInInventory += OnAfterItemPlacedInInventory;
    }

    private void OnAfterItemPlacedInInventory()
    {
        TutorialEvents.OnItemPlacedInInventory -= OnAfterItemPlacedInInventory;
        _inventoryCutout.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        TutorialEvents.OnItemPickedUpFromInventory += OnAfterItemPickedUpFromInventory;
    }

    private void OnAfterItemPickedUpFromInventory()
    {
        TutorialEvents.OnItemPickedUpFromInventory -= OnAfterItemPickedUpFromInventory;
        _background.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(false);
        _tutorialPlayer.MoveToNextNarratorText();
        TutorialEvents.OnItemDropped += OnAfterItemDrop;
    }

    private void OnAfterItemDrop()
    {
        TutorialEvents.OnItemDropped -= OnAfterItemDrop;
        OnActionFinishedInvoke();
    }

    public override void Exit()
    {
        TutorialManager.Instance.CanUseItem = true;
        TutorialManager.Instance.CurrentItemInRange = null;
        TutorialManager.Instance.IsPaused = false;
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class TutorialItemInteractionsAction : TutorialAction
{
    [SerializeField] private Image _background;

    [SerializeField] private Image _itemOnGroundCutout;
    [SerializeField] private Image _inventoryCutout;
    [SerializeField] private Image _itemCutout;

    private event Action CurrentMouseClickAction;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentMouseClickAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        CurrentMouseClickAction = null;
    }

    public override void StartAction()
    {
        Time.timeScale = 0f;
        _background.gameObject.SetActive(true);
        _itemOnGroundCutout.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        TutorialEvents.OnItemPickedUp += OnAfterItemPickedUp;
    }

    private void OnAfterItemPickedUp()
    {
        _itemOnGroundCutout.gameObject.SetActive(false);
        _inventoryCutout.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        TutorialEvents.OnItemPlacedInInventory += OnAfterItemPlacedInInventory;
    }

    private void OnAfterItemPlacedInInventory()
    {
        TutorialEvents.OnItemPlacedInInventory -= OnAfterItemPlacedInInventory;
        _inventoryCutout.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        TutorialEvents.OnItemPickedUpFromInventory -= OnAfterItemPickedUpFromInventory;
    }

    private void OnAfterItemPickedUpFromInventory()
    {
        TutorialEvents.OnItemPickedUpFromInventory -= OnAfterItemPickedUpFromInventory;
        _background.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(false);
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterItemDrop;
    }

    private void OnAfterItemDrop()
    {
        CurrentMouseClickAction = null;
        OnActionFinishedInvoke();
    }

    public override void Exit()
    {
        TutorialManager.Instance.InstantiateTutorial(TutorialManager.Instance.CurrentItemInRange.Stats.ItemType);
        Time.timeScale = 1f;
    }
}

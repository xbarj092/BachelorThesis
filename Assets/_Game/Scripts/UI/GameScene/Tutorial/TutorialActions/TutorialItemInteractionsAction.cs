using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialItemInteractionsAction : TutorialAction
{
    [SerializeField] private InputActionReference _leftClickAction;
    [SerializeField] private InputActionReference _rightClickAction;
    [SerializeField] private InputActionReference _dropAction;

    [SerializeField] private Image _background;

    [SerializeField] private Image _itemOnGroundCutout;
    [SerializeField] private Image _inventoryCutout;
    [SerializeField] private Image _itemCutout;

    [SerializeField] private RectTransform _itemOnGroundTransform;
    [SerializeField] private RectTransform _inventoryTransform;
    [SerializeField] private RectTransform _itemTransform;

    private CinemachineVirtualCamera _cinemachineCamera;
    private Transform _player;

    private void OnDisable()
    {
        TutorialManager.Instance.IsPaused = false;
        TutorialManager.Instance.CanUseItem = true;
        TutorialManager.Instance.CanHighlightItem = true;
        TutorialEvents.OnItemPickedUp -= OnAfterItemPickedUp;
        TutorialEvents.OnItemPlacedInInventory -= OnAfterItemPlacedInInventory;
        TutorialEvents.OnItemPickedUpFromInventory -= OnAfterItemPickedUpFromInventory;
        TutorialEvents.OnItemDropped -= OnAfterItemDrop;
    }

    public override void StartAction()
    {
        _cinemachineCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        _player = FindFirstObjectByType<Player>().transform;

        TutorialManager.Instance.IsPaused = true;
        TutorialManager.Instance.CanUseItem = false;
        _background.gameObject.SetActive(true);
        _cinemachineCamera.m_Follow = TutorialManager.Instance.CurrentItemInRange.transform;
        StartCoroutine(DelayedItemHighlight());
    }

    private IEnumerator DelayedItemHighlight()
    {
        yield return new WaitForSeconds(1);
        TutorialManager.Instance.CurrentItemInRange.Highlight();
        _itemOnGroundCutout.sprite = TutorialManager.Instance.CurrentItemInRange.Stats.Sprite;
        TutorialManager.Instance.CanHighlightItem = false;
        TutorialManager.Instance.CanUseItem = true;
        _itemOnGroundCutout.transform.position = Camera.main.WorldToScreenPoint(TutorialManager.Instance.CurrentItemInRange.transform.position);
        _itemOnGroundCutout.gameObject.SetActive(true);
        _tutorialPlayer.SetTextTransform(_itemOnGroundTransform);
        _tutorialPlayer.MoveToNextNarratorText();
        _tutorialPlayer.PublicText.text = string.Format(_tutorialPlayer.PublicText.text, _leftClickAction.action.GetBindingDisplayString());
        TutorialEvents.OnItemPickedUp += OnAfterItemPickedUp;
    }

    private void OnAfterItemPickedUp()
    {
        TutorialEvents.OnItemPickedUp -= OnAfterItemPickedUp;
        _cinemachineCamera.m_Follow = _player;
        _itemOnGroundCutout.gameObject.SetActive(false);
        _inventoryCutout.gameObject.SetActive(true);
        _tutorialPlayer.SetTextTransform(_inventoryTransform);
        _tutorialPlayer.MoveToNextNarratorText();
        _tutorialPlayer.PublicText.text = string.Format(_tutorialPlayer.PublicText.text, _rightClickAction.action.GetBindingDisplayString());
        TutorialManager.Instance.CanUseItem = false;
        TutorialManager.Instance.CanDropItem = false;
        TutorialManager.Instance.CanHighlightItem = true;
        TutorialEvents.OnItemPlacedInInventory += OnAfterItemPlacedInInventory;
    }

    private void OnAfterItemPlacedInInventory()
    {
        TutorialEvents.OnItemPlacedInInventory -= OnAfterItemPlacedInInventory;
        _inventoryCutout.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(true);
        _tutorialPlayer.SetTextTransform(_itemTransform);
        _tutorialPlayer.MoveToNextNarratorText();
        _tutorialPlayer.PublicText.text = string.Format(_tutorialPlayer.PublicText.text, _rightClickAction.action.GetBindingDisplayString());
        TutorialEvents.OnItemPickedUpFromInventory += OnAfterItemPickedUpFromInventory;
    }

    private void OnAfterItemPickedUpFromInventory()
    {
        TutorialEvents.OnItemPickedUpFromInventory -= OnAfterItemPickedUpFromInventory;
        _background.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(false);
        _tutorialPlayer.MoveToNextNarratorText();
        _tutorialPlayer.PublicText.text = string.Format(_tutorialPlayer.PublicText.text, _dropAction.action.GetBindingDisplayString());
        TutorialManager.Instance.CanDropItem = true;
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

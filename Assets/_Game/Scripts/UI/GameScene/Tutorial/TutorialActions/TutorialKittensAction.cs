using AYellowpaper.SerializedCollections;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialKittensAction : TutorialAction
{
    [SerializeField] private InputActionReference _pickUpAction;
    [SerializeField] private InputActionReference _useAction;

    [Space(5)]
    [SerializeField] private Image _background;
    [SerializeField] private GameObject _continueButton;

    [Space(5)]
    [SerializeField] private Image _kittenCutout;
    [SerializeField] private Image _itemCutout;
    [SerializeField] private Image _kittenAreaCutout;
    [SerializeField] private Image _areaBetweenPlayerKittenCutout;
    [SerializeField] private Image _noiseCutout;

    [Space(5)]
    [SerializeField] private RectTransform _kittenTransform;
    [SerializeField] private RectTransform _noItemTransform;
    [SerializeField] private RectTransform _itemTransform;

    [Space(5)]
    [SerializeField] private SerializedDictionary<ItemType, List<string>> _itemStrings;

    private List<string> _strings = new();
    private int _index = 0;

    private CinemachineVirtualCamera _cinemachineCamera;
    private Transform _player;

    private event Action _currentMouseClickAction;
    private event Action _nextAction;

    private const float ITEM_HIGHLIGHT_OFFSET = 116.66667f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ScreenManager.Instance.ActiveGameScreen == null)
        {
            _currentMouseClickAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        TutorialManager.Instance.IsPaused = false;
        TutorialManager.Instance.CanUseItem = true;
        TutorialManager.Instance.CanDropItem = true;
        _currentMouseClickAction = null;
        TutorialManager.Instance.CurrentKittenInRange.Unhighlight();
        TutorialEvents.OnScrolledToItem -= OnScrolledToItem;
        TutorialEvents.OnItemPickedUpFromInventory -= OnItemPickedUpFromInventory;
        TutorialEvents.OnItemUsed -= OnItemUsed;
        StopAllCoroutines();
    }

    public override void StartAction()
    {
        _cinemachineCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        _player = FindFirstObjectByType<Player>().transform;
        _cinemachineCamera.m_Follow = TutorialManager.Instance.CurrentKittenInRange.transform;

        TutorialManager.Instance.IsPaused = true;
        TutorialManager.Instance.CanUseItem = false;
        TutorialManager.Instance.CanDropItem = false;
        _background.gameObject.SetActive(true);
        StartCoroutine(DelayedItemHighlight());
    }

    private IEnumerator DelayedItemHighlight()
    {
        yield return new WaitForSeconds(1f);
        _kittenCutout.transform.position = Camera.main.WorldToScreenPoint(TutorialManager.Instance.CurrentKittenInRange.transform.position);
        _kittenCutout.gameObject.SetActive(true);
        TutorialManager.Instance.CurrentKittenInRange.Highlight();
        _tutorialPlayer.MoveToNextNarratorText();
        _tutorialPlayer.SetTextTransform(_kittenTransform);
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterKittenHighlighted;
    }

    private void OnAfterKittenHighlighted()
    {
        _currentMouseClickAction = null;
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterInteractionExplained;
    }

    private void OnAfterInteractionExplained()
    {
        _currentMouseClickAction = null;
        _kittenCutout.gameObject.SetActive(false);
        _cinemachineCamera.m_Follow = _player;
        _continueButton.gameObject.SetActive(false);
        _tutorialPlayer.PublicText.gameObject.SetActive(false);
        StartCoroutine(DelayedOnAfterInteractionExplained());
    }

    private IEnumerator DelayedOnAfterInteractionExplained()
    {
        yield return new WaitForSeconds(1f);
        TutorialManager.Instance.CanSeeNoise = true;
        PlayerNoise playerNoise = FindFirstObjectByType<PlayerNoise>();
        playerNoise.UnlockNoise();
        _noiseCutout.transform.localScale = playerNoise.transform.localScale / 2;
        _noiseCutout.gameObject.SetActive(true);
        _tutorialPlayer.PublicText.gameObject.SetActive(true);
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterNoiseShown;
    }

    private void OnAfterNoiseShown()
    {
        _currentMouseClickAction = null;
        _noiseCutout.gameObject.SetActive(false);
        _kittenCutout.gameObject.SetActive(true);
        _kittenCutout.transform.position = Camera.main.WorldToScreenPoint(TutorialManager.Instance.CurrentKittenInRange.transform.position);
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterNoHitNotified;
    }

    private void OnAfterNoHitNotified()
    {
        _currentMouseClickAction = null;
        _tutorialPlayer.MoveToNextNarratorText();
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterMatingNotified;
    }

    private void OnAfterMatingNotified()
    {
        _currentMouseClickAction = null;
        InventoryData inventoryData = LocalDataStorage.Instance.PlayerData.InventoryData;

        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            if (inventoryData.ItemsInInventory.Any(item => item != null && item.Stats.ItemType == itemType))
            {
                PromptItemUse(inventoryData, itemType);
                return;
            }
        }

        NoItemsInInventory();
    }

    private void PromptItemUse(InventoryData inventoryData, ItemType itemType)
    {
        for (int i = 0; i < inventoryData.ItemsInInventory.Count; i++)
        {
            if (inventoryData.ItemsInInventory[i] != null && inventoryData.ItemsInInventory[i].Stats.ItemType == itemType)
            {
                _index = i;
                OnItemFound(_itemStrings[itemType]);
                return;
            }
        }
    }

    private void OnItemFound(List<string> strings)
    {
        _kittenCutout.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(true);
        TutorialManager.Instance.CanUseItem = true;
        TutorialManager.Instance.CanDropItem = false;
        _itemCutout.transform.localPosition += new Vector3(ITEM_HIGHLIGHT_OFFSET * _index, 0);
        _tutorialPlayer.SetTextTransform(_itemTransform);
        _tutorialPlayer.SetTextLocalPosition(_tutorialPlayer.PublicText.transform.localPosition + new Vector3(ITEM_HIGHLIGHT_OFFSET * _index, 0));
        _strings = strings;
        TutorialManager.Instance.CurrentItemToUse = LocalDataStorage.Instance.PlayerData.InventoryData.ItemsInInventory[_index];
        _tutorialPlayer.PublicText.text = _strings[0];
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterItemFound;
    }

    private void OnAfterItemFound()
    {
        _continueButton.gameObject.SetActive(false);
        _currentMouseClickAction = null;
        if (LocalDataStorage.Instance.PlayerData.InventoryData.CurrentHighlightIndex != _index)
        {
            _tutorialPlayer.PublicText.text = _strings[1];
            TutorialEvents.OnScrolledToItem += OnScrolledToItem;
        }
        else
        {
            OnScrolledToItem();
        }
    }

    private void OnScrolledToItem()
    {
        TutorialEvents.OnScrolledToItem -= OnScrolledToItem;
        _tutorialPlayer.PublicText.text = _strings[2];
        _tutorialPlayer.PublicText.text = string.Format(_tutorialPlayer.PublicText.text, _pickUpAction.action.GetBindingDisplayString());
        TutorialEvents.OnItemPickedUpFromInventory += OnItemPickedUpFromInventory;
    }

    private void OnItemPickedUpFromInventory()
    {
        TutorialEvents.OnItemPickedUpFromInventory -= OnItemPickedUpFromInventory;
        _itemCutout.gameObject.SetActive(false);
        _tutorialPlayer.PublicText.text = _strings[3];
        _cinemachineCamera.m_Follow = TutorialManager.Instance.CurrentKittenInRange.transform;
        StartCoroutine(DelayedAfterPick());
    }

    private IEnumerator DelayedAfterPick()
    {
        TutorialManager.Instance.CanUseItem = false;
        TutorialManager.Instance.CanDropItem = false;
        _tutorialPlayer.PublicText.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        TutorialManager.Instance.CanUseItem = true;

        switch (TutorialManager.Instance.CurrentItemToUse.Stats.ItemType)
        {
            case ItemType.Laser:
            case ItemType.Mouse:
                _kittenAreaCutout.gameObject.SetActive(true);
                break;
            case ItemType.CardboardBox:
                _areaBetweenPlayerKittenCutout.gameObject.SetActive(true);
                _areaBetweenPlayerKittenCutout.transform.position = Camera.main.WorldToScreenPoint((_player.transform.position + TutorialManager.Instance.CurrentKittenInRange.transform.position) / 2);
                break;
            default:
                _kittenCutout.gameObject.SetActive(true);
                break;
        }

        _tutorialPlayer.PublicText.gameObject.SetActive(true);
        _tutorialPlayer.SetTextTransform(_kittenTransform);
        _tutorialPlayer.PublicText.text = string.Format(_tutorialPlayer.PublicText.text, _useAction.action.GetBindingDisplayString());
        TutorialEvents.OnItemUsed += OnItemUsed;
    }

    private void OnItemUsed()
    {
        TutorialEvents.OnItemUsed -= OnItemUsed;
        TutorialManager.Instance.CanUseItem = false;
        TutorialManager.Instance.CanDropItem = false;
        _background.gameObject.SetActive(false);
        _cinemachineCamera.m_Follow = _player;
        _kittenCutout.gameObject.SetActive(false);
        _kittenAreaCutout.gameObject.SetActive(false);
        _areaBetweenPlayerKittenCutout.gameObject.SetActive(false);
        _tutorialPlayer.PublicText.text = _strings[4];

        if (TutorialManager.Instance.CurrentItemToUse.Stats.ItemType == ItemType.CardboardBox)
        {
            if (!((CardboardBox)TutorialManager.Instance.CurrentItemToUse).HasKitten)
            {
                _tutorialPlayer.PublicText.text = _strings[5];
            }
        }

        _currentMouseClickAction = null;
        TutorialManager.Instance.CurrentKittenInRange.Unhighlight();
        StartCoroutine(DelayedOnItemUsed());
    }

    private IEnumerator DelayedOnItemUsed()
    {
        yield return new WaitForSeconds(0.25f);
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterItemUsed;
    }

    private void OnAfterItemUsed()
    {
        _currentMouseClickAction = null;
        OnActionFinishedInvoke();
    }

    private void NoItemsInInventory()
    {
        _tutorialPlayer.MoveToNextNarratorText();
        _tutorialPlayer.SetTextTransform(_noItemTransform);
        TutorialManager.Instance.CurrentKittenInRange.Unhighlight();
        _kittenCutout.gameObject.SetActive(false);
        _background.gameObject.SetActive(false);
        StartCoroutine(DelayedClickToContinue());
        _currentMouseClickAction = WaitSkip;
        _nextAction = OnAfterSilentWalkNotified;
    }

    private void OnAfterSilentWalkNotified()
    {
        _currentMouseClickAction = null;
        OnActionFinishedInvoke();
    }

    private void WaitSkip()
    {
        OnAfterWaitTime();
        StopCoroutine(DelayedClickToContinue());
    }

    private IEnumerator DelayedClickToContinue()
    {
        _continueButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(2);
        OnAfterWaitTime();
    }

    private void OnAfterWaitTime()
    {
        if (_nextAction != null)
        {
            _continueButton.gameObject.SetActive(true);
            _currentMouseClickAction = new(_nextAction);
            _nextAction = null;
        }
    }

    public override void Exit()
    {
        TutorialManager.Instance.CanUseItem = true;
        TutorialManager.Instance.CanDropItem = true;
    }
}

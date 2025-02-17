using AYellowpaper.SerializedCollections;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class TutorialKittensAction : TutorialAction
{
    [SerializeField] private Image _background;

    [SerializeField] private Image _kittenCutout;
    [SerializeField] private Image _itemCutout;

    [SerializeField] private SerializedDictionary<ItemType, List<string>> _itemStrings;

    private List<string> _strings = new();

    private CinemachineVirtualCamera _cinemachineCamera;
    private Transform _player;

    private event Action CurrentMouseClickAction;

    private const float ITEM_HIGHLIGHT_OFFSET = 116.66667f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentMouseClickAction?.Invoke();
        }
    }

    private void OnDisable()
    {
        TutorialManager.Instance.IsPaused = false;
        CurrentMouseClickAction = null;
        TutorialEvents.OnItemPickedUpFromInventory -= OnItemPickedUpFromInventory;
        TutorialEvents.OnItemUsed -= OnItemUsed;
    }

    public override void StartAction()
    {
        _cinemachineCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        _player = FindFirstObjectByType<Player>().transform;
        _cinemachineCamera.m_Follow = TutorialManager.Instance.CurrentKittenInRange.transform;

        TutorialManager.Instance.IsPaused = true;
        _background.gameObject.SetActive(true);
        StartCoroutine(DelayedItemHighlight());
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterKittenHighlighted;
    }

    private IEnumerator DelayedItemHighlight()
    {
        yield return new WaitForSeconds(0.5f);
        _kittenCutout.transform.position = Camera.main.WorldToScreenPoint(TutorialManager.Instance.CurrentKittenInRange.transform.position);
        _kittenCutout.gameObject.SetActive(true);
    }

    private void OnAfterKittenHighlighted()
    {
        CurrentMouseClickAction = null;
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterNoHitNotified;
    }

    private void OnAfterNoHitNotified()
    {
        CurrentMouseClickAction = null;
        _tutorialPlayer.MoveToNextNarratorText();
        CurrentMouseClickAction = OnAfterMatingNotified;
    }

    private void OnAfterMatingNotified()
    {
        _cinemachineCamera.m_Follow = _player;
        CurrentMouseClickAction = null;
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
                OnItemFound(_itemStrings[itemType], i);
                return;
            }
        }
    }

    private void OnItemFound(List<string> strings, int index)
    {
        // highlight item slot on the index
        _kittenCutout.gameObject.SetActive(false);
        _itemCutout.gameObject.SetActive(true);
        _itemCutout.transform.position += new Vector3(ITEM_HIGHLIGHT_OFFSET * index, 0);
        _strings = strings;
        _tutorialPlayer.PublicText.text = _strings[0];
        TutorialManager.Instance.CurrentItemToUse = LocalDataStorage.Instance.PlayerData.InventoryData.ItemsInInventory[index];
        TutorialEvents.OnItemPickedUpFromInventory += OnItemPickedUpFromInventory;
    }

    private void OnItemPickedUpFromInventory()
    {
        TutorialEvents.OnItemPickedUpFromInventory -= OnItemPickedUpFromInventory;
        _kittenCutout.gameObject.SetActive(true);
        _itemCutout.gameObject.SetActive(false);
        TutorialEvents.OnItemUsed += OnItemUsed;
    }

    private void OnItemUsed()
    {
        TutorialEvents.OnItemUsed -= OnItemUsed;
        _kittenCutout.gameObject.SetActive(false);
        _background.gameObject.SetActive(false);
        _tutorialPlayer.PublicText.text = _strings[1];
        CurrentMouseClickAction = null;
        StartCoroutine(DelayedOnItemUsed());
    }

    private IEnumerator DelayedOnItemUsed()
    {
        yield return new WaitForSeconds(0.25f);
        CurrentMouseClickAction = OnAfterItemUsed;
    }

    private void OnAfterItemUsed()
    {
        CurrentMouseClickAction = null;
        OnActionFinishedInvoke();
    }

    private void NoItemsInInventory()
    {
        _tutorialPlayer.MoveToNextNarratorText();
        _kittenCutout.gameObject.SetActive(false);
        _background.gameObject.SetActive(false);
        CurrentMouseClickAction = OnAfterSilentWalkNotified;
    }

    private void OnAfterSilentWalkNotified()
    {
        CurrentMouseClickAction = null;
        OnActionFinishedInvoke();
    }

    public override void Exit()
    {
    }
}

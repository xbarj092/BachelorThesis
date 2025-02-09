using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private List<Sprite> _characterSprites;

    private Vector2 _moveInput;

    private void Awake()
    {
        _spriteRenderer.sprite = _characterSprites[LocalDataStorage.Instance.PlayerData.PlayerStats.SpriteIndex];
    }

    private void Start()
    {
        InvokeRepeating(nameof(DepleteFood), 1, 1);
    }

    private void OnEnable()
    {
        DataEvents.OnDataSaved += SaveData;
    }

    private void OnDisable()
    {
        DataEvents.OnDataSaved -= SaveData;
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = _moveInput;

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            float targetAngle = Mathf.Atan2(_moveInput.y, _moveInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, targetAngle + 90);
        }
    }

    private void SaveData()
    {
        LocalDataStorage.Instance.PlayerData.PlayerTransform = new(transform);
        SaveInventoryItems();
    }

    public void SaveInventoryItems()
    {
        LocalDataStorage.Instance.PlayerData.SavedInventoryData.SavedItems.Clear();

        foreach (UseableItem item in LocalDataStorage.Instance.PlayerData.InventoryData.ItemsInInventory)
        {
            if (item != null)
            {
                item.SaveInventoryItem();
            }
        }

        LocalDataStorage.Instance.PlayerData.SavedInventoryData.CurrentHighlightIndex = LocalDataStorage.Instance.PlayerData.InventoryData.CurrentHighlightIndex;
    }

    private void OnMove(InputValue inputValue)
    {
        _moveInput = inputValue.Get<Vector2>();
    }

    private void DepleteFood()
    {
        if (!GameManager.Instance.MapInitialized)
        {
            return;
        }

        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        stats.CurrentTimeLeft--;
        LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
    }

    public void EatFood(int timeStolen)
    {
        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        if (stats.CurrentTimeLeft - timeStolen > 0)
        {
            stats.CurrentTimeLeft -= timeStolen;
            LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
        }
        else
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        int timeAlive = LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive;
        UGSAnalyticsManager.Instance.RecordPlayerDeath(timeAlive);
        yield return StartCoroutine(LootLockerManager.Instance.SubmitScore(timeAlive));
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Death);

        // TODO: death
    }
}

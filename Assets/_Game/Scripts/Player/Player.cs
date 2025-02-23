using DG.Tweening;
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

    [Header("Field of View Settings")]
    [SerializeField] private float _viewRange = 12f;
    [SerializeField] private float _viewAngle = 60f;

    private bool _invincible;
    private Vector2 _moveInput;
    private int _frameCounter = 0;

    private float _timeBetweenSteps = 0.5f;
    private float _currentTime = 0f;

    public bool IsInvincible = false;

    public event Action OnTutorialStarted;

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
        if (TutorialManager.Instance.IsPaused)
        {
            _rigidbody.velocity = Vector2.zero;
            return;
        }

        _rigidbody.velocity = _moveInput;

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            if (TutorialManager.Instance.IsTutorialPlaying(TutorialID.Movement))
            {
                TutorialEvents.OnPlayerMovedInvoke();
            }

            _currentTime += Time.fixedDeltaTime;
            if (_currentTime >= _timeBetweenSteps)
            {
                _currentTime = 0f;
                AudioManager.Instance.Play(SoundType.Footsteps);
            }

            float targetAngle = Mathf.Atan2(_moveInput.y, _moveInput.x) * Mathf.Rad2Deg + 90;
            if (targetAngle != transform.rotation.z)
            {
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
            }
        }

        _frameCounter++;
        if (_frameCounter >= 5)
        {
            CheckFieldOfView();
            _frameCounter = 0;
        }
    }

    private void CheckFieldOfView()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _viewRange);

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.transform == transform)
            {
                continue;
            }

            float distanceToTarget = Vector2.Distance(transform.position, hit.transform.position);

            Vector2 directionToTarget = (hit.transform.position - transform.position).normalized;

            int layerMask = LayerMask.GetMask(GlobalConstants.Layers.Map.ToString());
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, layerMask);

            if (rayHit.collider != null)
            {
                continue;
            }

            float angleToTarget = Vector2.Angle(transform.right, directionToTarget);

            if (distanceToTarget > _viewRange || angleToTarget > (_viewAngle + 90) / 2)
            {
                continue;
            }

            if (TutorialManager.Instance.TutorialsEnabled)
            {
                if (!TutorialManager.Instance.CompletedTutorialIDs.Contains(TutorialID.ItemInteractions) && !TutorialManager.Instance.IsTutorialPlaying(TutorialID.ItemInteractions))
                {
                    if (hit.TryGetComponent(out UseableItem item) && item.IsInteractable)
                    {
                        TutorialManager.Instance.InstantiateTutorial(TutorialID.ItemInteractions);
                        TutorialManager.Instance.CurrentItemInRange = item;
                    }
                }
                if (!TutorialManager.Instance.CompletedTutorialIDs.Contains(TutorialID.Kittens) && !TutorialManager.Instance.IsTutorialPlaying(TutorialID.Kittens))
                {
                    if (hit.TryGetComponent(out Kitten kitten))
                    {
                        OnTutorialStarted?.Invoke();
                        TutorialManager.Instance.InstantiateTutorial(TutorialID.Kittens);
                        TutorialManager.Instance.CurrentKittenInRange = kitten;
                    }
                }
            }
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
        if (!GameManager.Instance.MapInitialized || TutorialManager.Instance.IsPaused)
        {
            return;
        }

        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        stats.CurrentTimeToEatFood--;
        if (stats.CurrentTimeToEatFood <= 0)
        {
            EatFood();
            stats.CurrentTimeToEatFood = stats.TimeToEatFood;
        }

        LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
    }

    public void EatFood(Vector3 kittenPosition = default)
    {
        PlayerStats stats = LocalDataStorage.Instance.PlayerData.PlayerStats;
        if (stats.CurrentFood > 1)
        {
            stats.CurrentFood--;

            if (kittenPosition != default)
            {
                _spriteRenderer.DOColor(Color.red, 0.1f).SetLoops(6, LoopType.Yoyo).OnComplete(() =>
                {
                    _spriteRenderer.color = Color.white;
                });
                StartCoroutine(TemporaryInvincibility(0.5f));
                Vector2 knockbackDirection = (transform.position - kittenPosition).normalized;
                _rigidbody.DOMove(transform.position + (Vector3)(knockbackDirection * 0.25f), 0.1f);
            }

            LocalDataStorage.Instance.PlayerData.PlayerStats = stats;
        }
        else
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator TemporaryInvincibility(float duration)
    {
        IsInvincible = true;
        yield return new WaitForSeconds(duration);
        IsInvincible = false;
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

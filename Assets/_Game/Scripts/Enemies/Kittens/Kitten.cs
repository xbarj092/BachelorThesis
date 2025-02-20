using System.Collections;
using System.Linq;
using UnityEngine;

public class Kitten : Enemy
{
    [SerializeField] private StateMachineBrain _stateMachineBrain;

    // change it to scriptable
    [SerializeField] private int _minTimeToLive = 120;
    [SerializeField] private int _maxTimeToLive = 240;
    private float _timeToLive = 120f;
    private float _currentTimeToLive;

    [Header("Death Settings")]
    [SerializeField] private Sprite _deadSprite;
    [SerializeField] private Sprite _defaultSprite;
    [SerializeField] private Sprite _focusedSprite;
    [SerializeField] private Sprite _trappedSprite;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Material _outlineMaterial;

    [Header("Field of View Settings")]
    [SerializeField] private float _viewRange = 5f;
    [SerializeField] private float _viewAngle = 60f;

    public Rigidbody2D Rigidbody;
    public Kitten PotentialPartner;

    private Vector3 _movementDirection;
    public Vector3 MovementDirection => _movementDirection;
    private Vector3 _lastPosition;

    public int PartnerUID;
    public int UID;

    public bool IsCastrated;
    public bool Male;

    public bool IsDead;
    public bool IsInRangeOfPlayer;
    public bool CanSeeTarget;
    public bool IsApproaching;
    public bool IsMating;
    public bool AlreadyMated = true;
    public bool IsTrapped;
    public bool IsRunningAway;

    private float _matingTimeout = 20f;
    private float _currentMatingTimeout = 0f;
    private int _frameCounter = 0;

    private Transform _playerTransform;

    private Transform _currentTarget;

    private FocusTargetType _currentFocusType;

    protected override void Update()
    {
        if (TutorialManager.Instance.IsPaused)
        {
            return;
        }

        base.Update();
        Vector3 currentPosition = transform.position;
        _movementDirection = (currentPosition - _lastPosition).normalized;
        _lastPosition = currentPosition;

        _frameCounter++;

        if (_frameCounter % 5 != 0 || IsTrapped)
        {
            return;
        }

        if (AlreadyMated)
        {
            _currentMatingTimeout += Time.deltaTime;
            if (_currentMatingTimeout >= _matingTimeout)
            {
                _currentMatingTimeout = 0f;
                AlreadyMated = false;
            }
        }

        if (_playerTransform != null)
        {
            CheckFieldOfView();
        }
    }

    public void Init()
    {
        StartCoroutine(_stateMachineBrain.SetUpBrain(this));
        _timeToLive = Random.Range(_minTimeToLive, _maxTimeToLive);
        _currentTimeToLive = _timeToLive;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        InvokeRepeating(nameof(Starve), 1, 1);
    }

    public IEnumerator Init(StateType stateType)
    {
        yield return new WaitUntil(() => GameManager.Instance.MapInitialized);
        yield return null;
        StartCoroutine(_stateMachineBrain.SetUpBrain(this, stateType));

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        InvokeRepeating(nameof(Starve), 1, 1);
        _renderer.sprite = GetSprite();
    }

    private Sprite GetSprite()
    {
        if (IsDead) return _deadSprite;
        if (IsTrapped) return _trappedSprite;
        if (CanSeeTarget) return _focusedSprite;
        return _defaultSprite;
    }

    private void Starve()
    {
        if (TutorialManager.Instance.IsPaused)
        {
            return;
        }

        _currentTimeToLive -= 1;

        if (_currentTimeToLive <= 0)
        {
            IsDead = true;
            gameObject.SetActive(false);
            _renderer.sprite = _deadSprite;
        }
    }

    public void Trap(bool box = false)
    {
        IsTrapped = true;
        
        if (box)
        {
            gameObject.SetActive(false);
        }
        else
        {
            _renderer.sprite = _trappedSprite;
        }
    }

    public void Untrap()
    {
        Collider2D overlap = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask(GlobalConstants.Layers.Interact.ToString()));
        if (overlap != null && overlap.TryGetComponent(out CardboardBox box) && box.Used)
        {
            box.SetHasKitten(true);
            Trap(true);
            return;
        }

        IsTrapped = false;
        _renderer.sprite = _defaultSprite;
    }

    private void CheckFieldOfView()
    {
        if (!CanPerformActions() || IsApproaching)
        {
            ResetFocus();
            return;
        }

        _currentTarget = null;
        _currentFocusType = FocusTargetType.None;

        FocusTargetType highestPriority = FocusTargetType.None;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _viewRange);

        foreach (Collider2D hit in hits)
        {
            if (hit == null || hit.transform == transform)
            {
                continue;
            }

            FocusTargetType targetType = DetermineTargetType(hit.gameObject);
            if (targetType == FocusTargetType.None)
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

            if (targetType == FocusTargetType.Player)
            {
                float angleToTarget = Vector2.Angle(transform.right, directionToTarget);

                if (distanceToTarget > _viewRange || angleToTarget > _viewAngle / 2)
                {
                    continue;
                }

            }

            if (IsHigherPriority(targetType, highestPriority))
            {
                _currentTarget = hit.transform;
                _currentFocusType = targetType;
                highestPriority = targetType;
            }
        }

        UpdateFocusState();
    }

    private FocusTargetType DetermineTargetType(GameObject target)
    {
        if (target.TryGetComponent(out UseableItem item) && item.Stats.ItemType == ItemType.Mouse && item.Used)
        {
            return FocusTargetType.Mouse;
        }

        if (target.CompareTag(GlobalConstants.Tags.Player.ToString()) && !LocalDataStorage.Instance.PlayerData.PlayerStats.StatusEffects.Any(effect => effect.Type == (int)StatusEffectType.Invisibility))
        {
            return FocusTargetType.Player;
        }

        if (target.TryGetComponent(out item) && item.Stats.ItemType == ItemType.Laser && item.Used)
        {
            return FocusTargetType.Laser;
        }

        return FocusTargetType.None;
    }

    private bool IsHigherPriority(FocusTargetType newType, FocusTargetType currentType)
    {
        if (currentType == FocusTargetType.Mouse)
        {
            return false;
        }
        if (currentType == FocusTargetType.Laser && newType == FocusTargetType.Mouse)
        {
            return true;
        }
        if (currentType == FocusTargetType.Player && (newType == FocusTargetType.Mouse || newType == FocusTargetType.Laser))
        {
            return true;
        }

        return currentType == FocusTargetType.None;
    }

    private void UpdateFocusState()
    {
        if (_currentTarget == null)
        {
            ResetFocus();
            return;
        }

        switch (_currentFocusType)
        {
            case FocusTargetType.Mouse:
                _stateMachineBrain.MouseTransform = _currentTarget.transform;
                _stateMachineBrain.PlayerTransform = null;
                _stateMachineBrain.LaserTransform = null;
                _renderer.sprite = _focusedSprite;
                CanSeeTarget = true;
                break;

            case FocusTargetType.Player:
                _stateMachineBrain.MouseTransform = null;
                _stateMachineBrain.PlayerTransform = _currentTarget.transform;
                _stateMachineBrain.LaserTransform = null;
                _renderer.sprite = _focusedSprite;
                CanSeeTarget = true;
                break;

            case FocusTargetType.Laser:
                _stateMachineBrain.MouseTransform = null;
                _stateMachineBrain.PlayerTransform = null;
                _stateMachineBrain.LaserTransform = _currentTarget.transform;
                _renderer.sprite = _focusedSprite;
                CanSeeTarget = true;
                break;

            default:
                ResetFocus();
                break;
        }
    }

    public void FocusOnPlayer()
    {
        if (IsTrapped || LocalDataStorage.Instance.PlayerData.PlayerStats.StatusEffects.Any(effect => effect.Type == (int)StatusEffectType.Invisibility))
        {
            return;
        }

        _stateMachineBrain.MouseTransform = null;
        _stateMachineBrain.PlayerTransform = _playerTransform;
        _stateMachineBrain.LaserTransform = null;
        _renderer.sprite = _focusedSprite;
        CanSeeTarget = true;
    }

    private void ResetFocus()
    {
        _currentTarget = null;
        _currentFocusType = FocusTargetType.None;
        if (!IsTrapped)
        {
            _renderer.sprite = _defaultSprite;
        }
        CanSeeTarget = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CanPerformActions())
        {
            return;
        }

        if (collision.gameObject.CompareTag(GlobalConstants.Tags.Player.ToString()) &&
            collision.gameObject.TryGetComponent(out Player player) && !IsRunningAway)
        {
            UGSAnalyticsManager.Instance.RecordFoodStolen(LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);
            player.EatFood(_stats.StealAmount);
            _currentTimeToLive = _timeToLive;
            IsRunningAway = true;
        }

        if (collision.gameObject.TryGetComponent(out UseableItem item) && item.Stats.ItemType == ItemType.Mouse && item.Used)
        {
            ItemManager.Instance.SpawnedItems.Remove(item);
            Destroy(item.gameObject);
        }
    }

    public bool CanPerformActions()
    {
        return !IsDead && !IsTrapped && !IsRunningAway && !IsMating;
    }

    public bool CanMate()
    {
        return CanPerformActions() && !CanSeeTarget && !IsInRangeOfPlayer && !AlreadyMated;
    }

    public void IsInRange(bool inRange)
    {
        IsInRangeOfPlayer = inRange;
        if (inRange && IsHigherPriority(FocusTargetType.Player, _currentFocusType) && !LocalDataStorage.Instance.PlayerData.PlayerStats.StatusEffects.Any(effect => effect.Type == (int)StatusEffectType.Invisibility))
        {
            _currentTarget = _playerTransform;
            _currentFocusType = FocusTargetType.Player;
            UpdateFocusState();
        }
    }

    public SavedKitten Save()
    {
        return new(new(transform), new(_currentTarget), new(MovementDirection), new(_lastPosition),
            _currentTimeToLive, _currentMatingTimeout, UID, PotentialPartner == null ? 0 : PotentialPartner.UID, _stateMachineBrain.GetCurrentStateId(), (int)_currentFocusType,
            Male, IsCastrated, IsDead, IsInRangeOfPlayer, CanSeeTarget, IsApproaching,
            IsMating, AlreadyMated, IsTrapped, IsRunningAway, gameObject.activeInHierarchy);
    }

    public void Load(SavedKitten savedKitten)
    {
        savedKitten.CurrentTarget.ApplyToTransform(_currentTarget);

        _movementDirection = savedKitten.MovementDirection.ToVector3();
        _lastPosition = savedKitten.LastPosition.ToVector3();

        _currentTimeToLive = savedKitten.TimeToLive;
        _matingTimeout = savedKitten.MatingTimeout;

        StartCoroutine(Init((StateType)savedKitten.CurrentState));
    }

    public void Highlight()
    {
        _renderer.material = _outlineMaterial;
        _renderer.material.SetInt("_Outlined", 1);
    }

    public void Unhighlight()
    {
        _renderer.material.SetInt("_Outlined", 0);
        _renderer.material = _defaultMaterial;
    }
}

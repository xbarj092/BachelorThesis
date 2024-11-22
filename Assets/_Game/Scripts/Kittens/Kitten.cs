using UnityEngine;

public class Kitten : MonoBehaviour
{
    [SerializeField] private StateMachineBrain _stateMachineBrain;

    // change it to scriptable
    [SerializeField] private float _timeToLive = 120f;
    private float _currentTimeToLive;

    [Header("Death Settings")]
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite _deadSprite;

    [Header("Field of View Settings")]
    [SerializeField] private float _viewRange = 5f;
    [SerializeField] private float _viewAngle = 60f;

    public Rigidbody2D Rigidbody;

    public bool IsCastrated;

    public bool IsDead;
    public bool IsInRangeOfPlayer;
    public bool CanSeeTarget;
    public bool IsMating;
    public bool IsTrapped;
    public bool IsRunningAway;

    private Transform _playerTransform;

    private Transform _currentTarget;

    private FocusTargetType _currentFocusType;

    private void Start()
    {
        StartCoroutine(_stateMachineBrain.SetUpBrain(this));
        _currentTimeToLive = _timeToLive;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        // InvokeRepeating(nameof(Starve), 1, 1);
    }

    private void Update()
    {
        if (_playerTransform != null)
        {
            CheckFieldOfView();
        }
    }

    private void Starve()
    {
        _currentTimeToLive -= 1;

        if (_currentTimeToLive <= 0)
        {
            IsDead = true;
            _renderer.sprite = _deadSprite;
        }
    }

    private void CheckFieldOfView()
    {
        if (!CanPerformActions())
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

            if (targetType == FocusTargetType.Player)
            {
                Vector2 directionToTarget = (hit.transform.position - transform.position).normalized;
                float angleToTarget = Vector2.Angle(transform.right, directionToTarget);

                if (distanceToTarget > _viewRange || angleToTarget > _viewAngle / 2)
                {
                    continue;
                }

                int layerMask = ~LayerMask.GetMask(GlobalConstants.Layers.Kitten.ToString());
                RaycastHit2D rayHit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, layerMask);

                if (rayHit.collider == null || rayHit.collider != hit)
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
        if (target.TryGetComponent(out Item item) && item.Stats.ItemType == ItemType.Mouse && item.Used)
        {
            return FocusTargetType.Mouse;
        }

        if (target.CompareTag(GlobalConstants.Tags.Player.ToString()))
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
                CanSeeTarget = true;
                Debug.Log("Kitten focuses on a mouse!");
                break;

            case FocusTargetType.Player:
                _stateMachineBrain.MouseTransform = null;
                _stateMachineBrain.PlayerTransform = _currentTarget.transform;
                _stateMachineBrain.LaserTransform = null;
                CanSeeTarget = true;
                Debug.Log("Kitten focuses on the player!");
                break;

            case FocusTargetType.Laser:
                _stateMachineBrain.MouseTransform = null;
                _stateMachineBrain.PlayerTransform = null;
                _stateMachineBrain.LaserTransform = _currentTarget.transform;
                CanSeeTarget = true;
                Debug.Log("Kitten focuses on the laser!");
                break;

            default:
                ResetFocus();
                break;
        }
    }

    private void ResetFocus()
    {
        _currentTarget = null;
        _currentFocusType = FocusTargetType.None;
        CanSeeTarget = false;
        Debug.Log("Kitten has no focus.");
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
            player.EatFood(LocalDataStorage.Instance.PlayerData.PlayerStats);
            _currentTimeToLive = _timeToLive;
            IsRunningAway = true;
        }

        if (collision.gameObject.TryGetComponent(out Item item) && item.Stats.ItemType == ItemType.Mouse && item.Used)
        {
            Destroy(item.gameObject);
        }
    }

    private bool CanPerformActions()
    {
        return !IsDead && !IsTrapped && !IsRunningAway;
    }
}

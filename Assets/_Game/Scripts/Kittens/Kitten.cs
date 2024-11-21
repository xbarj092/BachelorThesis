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

    public bool IsDead;
    public bool IsInRangeOfPlayer;
    public bool CanSeePlayer;
    public bool IsMating;
    public bool IsTrapped;
    public bool IsRunningAway;

    private Transform _playerTransform;

    private void Start()
    {
        StartCoroutine(_stateMachineBrain.SetUpBrain(this));
        _currentTimeToLive = _timeToLive;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }

        InvokeRepeating(nameof(Starve), 1, 1);
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
            return;
        }

        Vector2 directionToPlayer = (_playerTransform.position - transform.position).normalized;

        float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
        if (distanceToPlayer > _viewRange)
        {
            return;
        }

        float angleToPlayer = Vector2.Angle(transform.right, directionToPlayer);
        if (angleToPlayer > _viewAngle / 2)
        {
            return;
        }

        int layerMask = ~LayerMask.GetMask(GlobalConstants.Layers.Kitten.ToString());
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, _viewRange, layerMask);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag(GlobalConstants.Tags.Player.ToString()))
            {
                CanSeePlayer = true;
            }
            else
            {
                CanSeePlayer = false;
            }
        }
        else
        {
            CanSeePlayer = false;
        }
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
    }

    private bool CanPerformActions()
    {
        return !IsDead && !IsTrapped && !IsRunningAway;
    }
}

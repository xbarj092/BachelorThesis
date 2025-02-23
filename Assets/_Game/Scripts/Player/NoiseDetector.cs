using DG.Tweening;
using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _noiseRenderer;
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private float _baseNoiseRadius = 1f;
    [SerializeField] private float _speedMultiplier = 2f;
    [SerializeField] private Ease _scaleEase = Ease.OutQuad;

    private float _targetScale;

    private void Awake()
    {
        _noiseRenderer.gameObject.SetActive(TutorialManager.Instance.CanSeeNoise);
    }

    private void Update()
    {
        if (!TutorialManager.Instance.CanSeeNoise)
        {
            return;
        }

        AdjustNoiseRadius();

        if (transform.localScale.x != _targetScale)
        {
            transform.DOScale(_targetScale, 0.1f).SetEase(_scaleEase);
        }
    }

    public void UnlockNoise()
    {
        _noiseRenderer.gameObject.SetActive(true);
    }

    private void AdjustNoiseRadius()
    {
        float speed = _rigidbody.velocity.magnitude;
        _targetScale = _baseNoiseRadius + (speed * _speedMultiplier);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Kitten kitten))
        {
            Vector2 direction = (kitten.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, kitten.transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask(GlobalConstants.Layers.Map.ToString()));
            kitten.IsInRange(hit.collider == null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Kitten kitten))
        {
            kitten.IsInRange(false);
        }
    }
}

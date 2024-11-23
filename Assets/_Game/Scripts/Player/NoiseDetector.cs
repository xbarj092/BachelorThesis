using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    [SerializeField] private CircleCollider2D _noiseCollider;
    [SerializeField] private Rigidbody2D _rigidbody;

    [SerializeField] private float _baseNoiseRadius = 1f;
    [SerializeField] private float _speedMultiplier = 2f;

    private void Update()
    {
        AdjustNoiseRadius();
    }

    private void AdjustNoiseRadius()
    {
        float speed = _rigidbody.velocity.magnitude;
        _noiseCollider.radius = _baseNoiseRadius + (speed * _speedMultiplier);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Kitten kitten))
        {
            Vector2 direction = (kitten.transform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, kitten.transform.position);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, LayerMask.GetMask(GlobalConstants.Layers.Map.ToString()));
            kitten.IsInRangeOfPlayer = hit.collider == null;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Kitten kitten))
        {
            kitten.IsInRangeOfPlayer = false;
        }
    }
}

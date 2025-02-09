using UnityEngine;

public class KittenChecker : MonoBehaviour
{
    [SerializeField] private Kitten _kitten;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Kitten otherKitten = collision.GetComponentInParent<Kitten>();

        float distanceToTarget = Vector2.Distance(transform.position, collision.transform.position);
        Vector2 directionToTarget = (collision.transform.position - transform.position).normalized;

        int layerMask = LayerMask.GetMask(GlobalConstants.Layers.Map.ToString());
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, layerMask);

        if (rayHit.collider != null)
        {
            return;
        }

        if (otherKitten.CanMate() && _kitten.CanMate() && !_kitten.IsApproaching && (!otherKitten.IsApproaching || otherKitten.PotentialPartner == _kitten))
        {
            _kitten.PotentialPartner = otherKitten;
            _kitten.IsApproaching = true;
        }
    }
}

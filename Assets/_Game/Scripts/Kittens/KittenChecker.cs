using UnityEngine;

public class KittenChecker : MonoBehaviour
{
    [SerializeField] private Kitten _kitten;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Kitten otherKitten = collision.GetComponentInParent<Kitten>();
        if (otherKitten.CanMate() && _kitten.CanMate())
        {
            _kitten.PotentialPartner = otherKitten;
            _kitten.IsApproaching = true;
        }
    }
}

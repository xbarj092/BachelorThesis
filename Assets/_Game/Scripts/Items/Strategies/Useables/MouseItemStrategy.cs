using System.Collections;
using UnityEngine;

public class MouseItemStrategy : UseableItemStrategy
{
    public override bool CanUse(UseableItem item)
    {
        if (TutorialManager.Instance.IsTutorialPlaying(TutorialID.Kittens))
        {
            float distance = Vector2.Distance(Input.mousePosition, Camera.main.WorldToScreenPoint(TutorialManager.Instance.CurrentKittenInRange.transform.position));

            if (distance > 400)
            {
                return false;
            }
        }

        return true;
    }

    public override void Use(UseableItem item)
    {
        Debug.Log("[MouseItemStrategy] - Used mouse!");
        PlaceOnMousePosition(item);
        MoveAwayFromPlayerSinusoid(item);
    }

    public override void PickUp(UseableItem item)
    {
        base.PickUp(item);
        Debug.Log("[MouseItemStrategy] - Picked up mouse!");
    }

    private void MoveAwayFromPlayerSinusoid(UseableItem item)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag(GlobalConstants.Tags.Player.ToString()).transform;
        Vector3 startPosition = item.transform.position;
        Vector3 directionAwayFromPlayer = (startPosition - playerTransform.position).normalized;

        if (item.isActiveAndEnabled)
        {
            item.StartCoroutine(SinusoidalMovementWithWallCheck(item, directionAwayFromPlayer));
        }
    }

    private IEnumerator SinusoidalMovementWithWallCheck(UseableItem item, Vector3 initialDirection)
    {
        float elapsedTime = 0f;

        float amplitude = 0.05f;
        float frequency = 5f;
        float speed = 2f;
        float duration = 10f;

        Vector3 direction = initialDirection;
        Vector3 perpendicularDirection = new Vector3(-direction.y, direction.x, 0).normalized;

        int wallLayerMask = LayerMask.GetMask(GlobalConstants.Layers.Map.ToString());

        while (elapsedTime < duration)
        {
            while (Time.timeScale == 0 || TutorialManager.Instance.IsTutorialPlaying(TutorialID.Kittens))
            {
                yield return null;
            }

            elapsedTime += Time.fixedDeltaTime;

            RaycastHit2D hit = Physics2D.Raycast(item.transform.position, direction, 0.2f, wallLayerMask);

            if (hit.collider != null)
            {
                Vector3 hitNormal = hit.normal;
                direction = Vector3.Reflect(direction, hitNormal).normalized;
                perpendicularDirection = new Vector3(-direction.y, direction.x, 0).normalized;
            }

            Vector3 linearOffset = speed * Time.fixedDeltaTime * direction;

            float sineOffset = Mathf.Sin(elapsedTime * frequency) * amplitude;
            Vector3 sineWave = perpendicularDirection * sineOffset;

            Vector3 movement = linearOffset + sineWave;
            item.transform.position += movement;

            if (movement != Vector3.zero)
            {
                float targetAngle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg + 180;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

                item.transform.rotation = Quaternion.Lerp(
                    item.transform.rotation,
                    targetRotation,
                    Time.fixedDeltaTime * 10f
                );
            }

            yield return null;
        }

        item.Used = false;
    }
}

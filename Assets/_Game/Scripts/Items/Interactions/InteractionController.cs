using System;
using System.Collections;
using UnityEngine;

public class InteractionControler : MonoBehaviour
{
    private Transform _playerTransform;

    public bool Interactable;
    private bool _interactable
    {
        get => Interactable;
        set
        {
            if (Interactable != value)
            {
                Interactable = value;
                if (value)
                {
                    OnItemInRangeInvoke();
                }
                else
                {
                    OnItemOutOfRangeInvoke();
                }
            }
        }
    }

    public event Action OnItemOutOfRange;
    private void OnItemOutOfRangeInvoke()
    {
        OnItemOutOfRange?.Invoke();
    }

    public event Action OnItemInRange;
    private void OnItemInRangeInvoke()
    {
        OnItemInRange?.Invoke();
    }

    private Coroutine _visibilityCheckCoroutine;

    private void Awake()
    {
        _playerTransform = FindFirstObjectByType<Player>().transform;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerInteraction"))
        {
            if (_visibilityCheckCoroutine == null)
            {
                _visibilityCheckCoroutine = StartCoroutine(VisibilityCheckRoutine());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerInteraction"))
        {
            if (_visibilityCheckCoroutine != null)
            {
                StopCoroutine(_visibilityCheckCoroutine);
                _visibilityCheckCoroutine = null;
            }
            _interactable = false;
        }
    }

    private IEnumerator VisibilityCheckRoutine()
    {
        while (true)
        {
            _interactable = CanSeeItem();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private bool CanSeeItem()
    {
        Vector2 itemPosition = transform.position;
        Vector2 playerPosition = _playerTransform.position;
        Vector2 direction = itemPosition - playerPosition;

        float distance = Vector2.Distance(playerPosition, itemPosition);

        RaycastHit2D hit = Physics2D.Raycast(playerPosition, direction, distance, LayerMask.GetMask(GlobalConstants.Layers.Map.ToString()));
        return hit.collider == null;
    }
}

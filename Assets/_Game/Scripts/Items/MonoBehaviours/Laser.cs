using System;
using UnityEngine;

public class Laser : Item
{
    [SerializeField] private GameObject _laserDot;
    [SerializeField] private LayerMask _layerMask;

    private float _battery;
    public float Battery
    {
        get => _battery;
        set
        {
            _battery = value;
            OnBatteryChanged?.Invoke(value);
        }
    }

    private Transform _playerTransform;

    public event Action<float> OnBatteryChanged;

    protected override void Awake()
    {
        base.Awake();
        Battery = ((LaserItem)Stats).Battery;
        _playerTransform = FindFirstObjectByType<Player>().transform;
    }

    private void Update()
    {
        if (!_pickedUp)
        {
            return;
        }

        if (Camera.main == null)
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 validPosition = GetValidPlacementPosition(_playerTransform.position, mousePosition);
        transform.position = validPosition;
    }

    public Vector3 GetValidPlacementPosition(Vector3 playerPosition, Vector3 mousePosition)
    {
        Vector3 direction = mousePosition - playerPosition;
        float distance = Mathf.Min(direction.magnitude, 3);
        direction.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(playerPosition, direction, distance, _layerMask);
        return hit.collider ? (Vector3)hit.point - direction * 0.1f : playerPosition + direction * distance;
    }

    public override void UseStart()
    {
        _spriteRenderer.enabled = false;
        base.UseStart();
        gameObject.SetActive(true);
        _laserDot.SetActive(true);
    }

    public override void UseStop()
    {
        _spriteRenderer.enabled = true;
        _laserDot.SetActive(false);
        gameObject.SetActive(false);
        base.UseStop();
    }
}

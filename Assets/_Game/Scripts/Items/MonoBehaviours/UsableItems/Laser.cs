using System;
using UnityEngine;

public class Laser : UseableItem
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
            OnBatteryChanged?.Invoke(value / ((LaserItem)Stats).Battery);
        }
    }

    private Transform _playerTransform;
    private LineRenderer _lineRenderer;

    public event Action<float> OnBatteryChanged;

    protected override void Awake()
    {
        base.Awake();
        Battery = ((LaserItem)Stats).Battery;
        _playerTransform = FindFirstObjectByType<Player>().transform;
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (!_pickedUp)
        {
            _lineRenderer.enabled = false;
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

        UpdateLineRenderer(_playerTransform.position, validPosition);
    }

    private void UpdateLineRenderer(Vector3 startPosition, Vector3 endPosition)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);
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
        _lineRenderer.enabled = false;
        gameObject.SetActive(false);
        base.UseStop();
    }

    public override void SaveItem()
    {
        SavedLaserItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy, Battery);
        LocalDataStorage.Instance.GameData.ItemData.SavedItems.Add(savedItem);
    }

    public override void SaveInventoryItem()
    {
        SavedLaserItem savedItem = new(new(transform), (int)Stats.ItemType, UID, Dropped, Used, gameObject.activeInHierarchy, Battery);
        LocalDataStorage.Instance.PlayerData.SavedInventoryData.SavedItems.Add(savedItem);
    }

    public override void LoadItem(SavedUseableItem item)
    {
        if ((ItemType)item.ItemType == Stats.ItemType)
        {
            if (item is SavedLaserItem laserItem)
            {
                laserItem.ApplyToItem(this);
            }
        }
    }
}

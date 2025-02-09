using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected EnemyStats _stats;

    private void Start()
    {
        _renderer.sprite = _stats.Sprite;
    }

    protected virtual void Update()
    {
        if (IsInCameraBounds())
        {
            LocalDataStorage.Instance.PlayerData.UnlockedCollectibleData.AddEnemies(_stats);
        }
    }

    private bool IsInCameraBounds()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1 && viewportPos.z > 0;
    }
}

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] protected EnemyStats _stats;

    private void Start()
    {
        _renderer.sprite = _stats.Sprite;
    }
}

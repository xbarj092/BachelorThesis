using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "EnemyStats", order = 0)]
public class EnemyStats : ScriptableObject, ICollectible
{
    [field: SerializeField] public string Title { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
    [field: SerializeField] public int StealAmount { get; set; }
    [field: SerializeField] public string Description { get; set; }
}

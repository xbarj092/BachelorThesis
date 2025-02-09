using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "EnemyStats", order = 0)]
public class EnemyStats : ScriptableObject
{
    public Sprite Sprite;
    public int StealAmount;
    public string Description;
}

using UnityEngine;

[CreateAssetMenu(fileName = "UseableItem", menuName = "Items/Useable", order = 0)]
public class UseableItem : ItemBase
{
    public float SpawnChance;
    public Sprite Sprite;
    public ItemType ItemType;
}

using UnityEngine;

public class ItemBaseSO : ScriptableObject, ICollectible
{
    [field: SerializeField] public string Title { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
    [field: SerializeField] public string Description { get; set; }
}

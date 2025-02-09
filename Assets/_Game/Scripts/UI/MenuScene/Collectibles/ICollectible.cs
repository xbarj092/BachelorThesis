using UnityEngine;

public interface ICollectible
{
    string Title { get;  set; }
    Sprite Sprite { get;  set; }
    string Description { get;  set; }
}

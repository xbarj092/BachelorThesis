using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public bool MapInitialized = false;
    public Vector2 StartRoomLocation = Vector2.zero;
}

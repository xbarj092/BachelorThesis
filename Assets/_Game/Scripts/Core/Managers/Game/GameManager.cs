using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public bool MapInitialized = false;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}

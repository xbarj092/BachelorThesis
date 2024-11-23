using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}

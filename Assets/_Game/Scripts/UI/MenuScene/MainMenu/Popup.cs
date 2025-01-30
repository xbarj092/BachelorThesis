using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private bool _destroyOnClose;

    public void Close()
    {
        if (_destroyOnClose)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

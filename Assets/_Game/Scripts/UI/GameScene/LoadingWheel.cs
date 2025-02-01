using UnityEngine;

public class LoadingWheel : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    private float _rotationSpeed = 360f;

    private void Update()
    {
        _rectTransform.Rotate(0f, 0f, -_rotationSpeed * Time.deltaTime);
    }

    public void Reset()
    {
        _rectTransform.rotation = Quaternion.identity;
    }
}

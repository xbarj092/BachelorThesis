using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image _fillImage;

    public void SetProgress(float progress)
    {
        _fillImage.fillAmount = progress;
    }
}

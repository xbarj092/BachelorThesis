using UnityEngine;
using DG.Tweening;

public class UIHighlighter : MonoBehaviour
{
    [SerializeField] private Vector3 _highlightedScale;
    [SerializeField] private float _animationTime;

    public virtual void Highlight()
    {
        transform.DOScale(_highlightedScale, _animationTime);
    }

    public virtual void Unhighlight()
    {
        transform.DOScale(Vector3.one, _animationTime);
    }
}

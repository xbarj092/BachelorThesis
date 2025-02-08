using UnityEngine;
using UnityEngine.UI;

public class ColoredUIHighlighter : UIHighlighter
{
    [SerializeField] private Graphic _targetGraphic;
    [SerializeField] private Color _highlightedColor;
    [SerializeField] private Color _unhighlightedColor;

    public override void Highlight()
    {
        _targetGraphic.color = _highlightedColor;
        base.Highlight();
    }

    public override void Unhighlight()
    {
        _targetGraphic.color = _unhighlightedColor;
        base.Unhighlight();
    }
}

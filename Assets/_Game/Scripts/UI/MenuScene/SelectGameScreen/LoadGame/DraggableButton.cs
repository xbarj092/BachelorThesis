using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableButton : Button, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect _scrollRect;

    protected override void Start()
    {
        base.Start();
        _scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _scrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _scrollRect.OnDrag(eventData);
        interactable = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _scrollRect.OnEndDrag(eventData);
        interactable = true;
    }
}

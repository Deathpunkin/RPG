using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragPanel : EventTrigger
{

    private bool dragging;
   

    public void Update()
    {
        var rectTransform = transform.parent.GetComponent<RectTransform>();
        float width = rectTransform.sizeDelta.x;
        float height = rectTransform.sizeDelta.y;
        if (dragging)
        {
            transform.parent.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.SetAsLastSibling();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        dragging = true;
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
         
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler {

    private Vector2 pointerOffset;
    private RectTransform canvasRectTransform;
    private RectTransform panelRectTransform;
    Canvas canvas;

    void awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if(canvas != null)
        {
            canvasRectTransform = canvas.transform as RectTransform;
            panelRectTransform = transform.parent as RectTransform;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        panelRectTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);
        Debug.Log(name + "Been Clicked");
    }

    public void OnDrag(PointerEventData data)
    {
        if (panelRectTransform = null)
        {
            return;
        }
        Vector2 localPointerPosition;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle (canvasRectTransform, data.position, data.pressEventCamera, out localPointerPosition))
        {
            panelRectTransform.localPosition = localPointerPosition - pointerOffset;
        }
    }
}

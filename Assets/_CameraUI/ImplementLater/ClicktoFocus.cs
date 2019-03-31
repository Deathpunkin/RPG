using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ClicktoFocus : EventTrigger {
    public void Update()
    {
        var rectTransform = transform.parent.GetComponent<RectTransform>();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        transform.parent.SetAsLastSibling();
    }
}

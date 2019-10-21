using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class Draggable : MonoBehaviour, IDragHandler
{
    public Action onDragged;
    
    private RectTransform rectTransform;
    private RectTransform parentRectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position += (Vector3) eventData.delta;

        var boundY = (parentRectTransform.sizeDelta.y - rectTransform.sizeDelta.y) / 2f;
        var boundX = (parentRectTransform.sizeDelta.x - rectTransform.sizeDelta.x) / 2f;

        var x = Mathf.Clamp(rectTransform.anchoredPosition.x, -boundX, boundX);
        var y = Mathf.Clamp(rectTransform.anchoredPosition.y, -boundY, boundY);
        
        rectTransform.anchoredPosition = new Vector2(x, y);
        
        onDragged?.Invoke();
    }
}
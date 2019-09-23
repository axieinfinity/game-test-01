using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.UI
{
    public class Interact : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        private bool dragging;
        private PointerEventData.InputButton draggingButton;
        private Vector2 dragStartPos, dragLastPos;

        [Space(32)]
        public OnCameraDrag onCameraDrag;
        [System.Serializable] public class OnCameraDrag : UnityEngine.Events.UnityEvent<Vector2> { }

        public OnCameraScroll onCameraScroll;
        [System.Serializable] public class OnCameraScroll : UnityEngine.Events.UnityEvent<int> { }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (dragging) return;
            draggingButton = eventData.button;
            dragging = true;
            dragStartPos = eventData.position;
            dragLastPos = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging == false || eventData.button != draggingButton) return;

            var diff = eventData.position - dragLastPos;
            diff.x /= Screen.width;
            diff.y /= Screen.height;
            dragLastPos = eventData.position;

            onCameraDrag?.Invoke(diff);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragging == false) return;
            if (eventData.button != draggingButton) return;

            dragging = false;
        }

        public void OnScroll(PointerEventData eventData)
        {
            onCameraScroll?.Invoke(eventData.scrollDelta.y > 0 ? 1 : -1);
        }
    }
}
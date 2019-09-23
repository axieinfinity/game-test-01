using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.UI
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private Camera minimapCam;
        [SerializeField] private RenderTexture minimapRenderTexture;
        [SerializeField] private CustomCameraControl customCameraControl;

        [Space(32)]
        [SerializeField] private Transform highlightArea;
        [SerializeField] private RectTransform minimapArea;

        [Space(32)]
        public OnMinimapSetCameraPosition onMinimapSetCameraPosition;
        [System.Serializable] public class OnMinimapSetCameraPosition : UnityEngine.Events.UnityEvent<Vector3> { }

        [Space(32)]
        public OnMinimapSetCameraScroll onMinimapSetCameraScroll;
        [System.Serializable] public class OnMinimapSetCameraScroll : UnityEngine.Events.UnityEvent<int> { }

        private Vector2 viewSize = Vector2.one;

        public void FunctionOnViewLimitChanged(Rect viewLimit)
        {
            var viewHeight = viewLimit.height / 2 + 2;
            var viewWidth = viewHeight / minimapRenderTexture.height * minimapRenderTexture.width;

            viewSize = new Vector2(viewWidth, viewHeight) * 2;
            minimapCam.targetTexture = minimapRenderTexture;
            minimapCam.orthographicSize = viewHeight;

            FunctionOnCustomCameraControlTransformChanged(customCameraControl);
        }

        public void FunctionOnCustomCameraControlTransformChanged(CustomCameraControl cam)
        {
            var camSizeHeight = cam.OrthographicSize;
            var miniCamSizeHeight = minimapCam.orthographicSize;

            highlightArea.localScale = Vector3.one * camSizeHeight / miniCamSizeHeight;
            highlightArea.localPosition = mainCam.transform.position / viewSize * minimapArea.rect.size;
        }

        public void OnMinimapDrag(UnityEngine.EventSystems.BaseEventData be)
        {
            var e = be as UnityEngine.EventSystems.PointerEventData;
            Vector2 minimapPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapArea, e.position, mainCam, out minimapPos);

            var pos = minimapPos / minimapArea.rect.size * viewSize;
            onMinimapSetCameraPosition?.Invoke(pos);
        }

        public void OnMinimapScroll(UnityEngine.EventSystems.BaseEventData be)
        {
            var e = be as UnityEngine.EventSystems.PointerEventData;
            onMinimapSetCameraScroll?.Invoke(e.scrollDelta.y > 0 ? 1 : -1);
        }
    }
}
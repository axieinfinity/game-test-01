using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class CustomCameraControl : MonoBehaviour
    {

        public float baseCameraSize = 12;
        [SerializeField] private Vector2 referenceCameraRatio;
        [SerializeField] private Camera targetCamera;

        private float zoomScale = 0;
        private float widthHeightRatio = 1;
        private Rect rectViewLimit;
        public float OrthographicSize;// { get; private set; }

        [System.Serializable] public class OnCameraTransformChanged : UnityEngine.Events.UnityEvent<CustomCameraControl> { }
        public OnCameraTransformChanged onCameraTransformChanged;

        private void Reset()
        {
            targetCamera = GetComponent<Camera>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (targetCamera == null) this.enabled = false;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            var ratio = Screen.width * 1f / Screen.height;
            var diff = Mathf.Abs(ratio - this.widthHeightRatio);
            if (diff > float.Epsilon) ResetCameraRatio();
        }

        public void SetViewLimit(Rect viewLimit)
        {
            this.rectViewLimit = viewLimit;
            ResetCameraRatio();
            onCameraTransformChanged?.Invoke(this);
        }

        void ClampCameraPos()
        {
            var pos = targetCamera.transform.localPosition;
            pos.x = Mathf.Clamp(pos.x, rectViewLimit.xMin, rectViewLimit.xMax);
            pos.y = Mathf.Clamp(pos.y, rectViewLimit.yMin, rectViewLimit.yMax);
            targetCamera.transform.localPosition = pos;
        }

        void ResetCameraRatio()
        {
            var ratio = Screen.width * 1f / Screen.height;
            this.widthHeightRatio = ratio;

            var targetWidthHeightRatio = this.referenceCameraRatio.x / this.referenceCameraRatio.y;
            var cameraSize = baseCameraSize * Mathf.Pow(1.2f, zoomScale);
            OrthographicSize = cameraSize;

            //if ratio is greater than layout is overflowing by width, apply baseCameraSize
            if (ratio >= targetWidthHeightRatio)
                targetCamera.orthographicSize = cameraSize;
            else
                targetCamera.orthographicSize = cameraSize / ratio * targetWidthHeightRatio;

            ClampCameraPos();
        }

        public void FunctionOnCameraDrag(Vector2 screenPercent)
        {
            var diff = Vector3.one;
            diff.x = screenPercent.x * targetCamera.orthographicSize / Screen.height * Screen.width;
            diff.y = screenPercent.y * targetCamera.orthographicSize;
            diff.z = 0;

            targetCamera.transform.localPosition -= diff * 2;

            ClampCameraPos();
            onCameraTransformChanged?.Invoke(this);
        }

        public void FunctionOnCameraMoved(Vector3 position)
        {
            targetCamera.transform.localPosition = position;
            ClampCameraPos();
            onCameraTransformChanged?.Invoke(this);
        }

        public void FunctionOnCameraZoom(int level)
        {
            zoomScale = Mathf.Clamp(zoomScale + level, -8, 20);
            ResetCameraRatio();
            onCameraTransformChanged?.Invoke(this);
        }
    }
}

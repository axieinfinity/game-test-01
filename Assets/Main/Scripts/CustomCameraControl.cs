using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCameraControl : MonoBehaviour
{

    public float baseCameraSize = 12;
    [SerializeField] private Vector2 referenceCameraRatio;
    [SerializeField] private Camera targetCamera;

    float zoomScale = 0;
    float widthHeightRatio = 1;

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

    void ResetCameraRatio()
    {
        var ratio = Screen.width * 1f / Screen.height;
        this.widthHeightRatio = ratio;

        var targetWidthHeightRatio = this.referenceCameraRatio.x / this.referenceCameraRatio.y;
        var cameraSize = baseCameraSize * Mathf.Pow(1.2f, zoomScale);

        //if ratio is greater than layout is overflowing by width, apply baseCameraSize
        if (ratio >= targetWidthHeightRatio)
            targetCamera.orthographicSize = cameraSize;
        else
            targetCamera.orthographicSize = cameraSize / ratio * targetWidthHeightRatio;
    }

    public void FunctionOnCameraDrag(Vector2 screenPercent)
    {
        var diff = Vector3.one;
        diff.x = screenPercent.x * targetCamera.orthographicSize / Screen.height * Screen.width;
        diff.y = screenPercent.y * targetCamera.orthographicSize;
        diff.z = 0;

        targetCamera.transform.localPosition -= diff * 2;
    }

    public void FunctionOnCameraZoom(int level)
    {
        zoomScale += level;
        zoomScale = Mathf.Max(-8, zoomScale);
        ResetCameraRatio();
    }
}

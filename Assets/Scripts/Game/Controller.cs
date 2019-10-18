using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{   
    const float MIN_FOW_VALUE = 1f;
    const float STEP_FOW_VALUE = 10f;
    [SerializeField] Camera gameCamera = null;
    enum CZoomType {
        None,
        Out,
        In
    }
    private CZoomType _zoomType = CZoomType.None;

    private void Start() {
        ZoomButton.onPress += OnZoomButtonPress;
    }

    private void OnDestroy() {
        ZoomButton.onPress -= OnZoomButtonPress;
    }

    public void ZoomOut(float amount)
    {
        gameCamera.fieldOfView += amount;
    }

    public void ZoomIn (float amount)
    {
        float newFieldOfView = gameCamera.fieldOfView - amount;
        if (newFieldOfView < MIN_FOW_VALUE)
        {
            newFieldOfView = MIN_FOW_VALUE;
        }
        gameCamera.fieldOfView = newFieldOfView;
    }

    void OnZoomButtonPress (ZoomButton target, bool isPressed) {
        if (target.Type == ZoomButton.BType.Out) {

            if (isPressed) {
                _zoomType = CZoomType.Out;
            } else {
                if (_zoomType == CZoomType.Out) {
                    _zoomType = CZoomType.None;
                }
            }
            return;
        }

        if (target.Type == ZoomButton.BType.In) {
            if (isPressed) {
                _zoomType = CZoomType.In;
            } else {
                if (_zoomType == CZoomType.In) {
                    _zoomType = CZoomType.None;
                }
            }
            return;
        }
    }
    private void Update() {
        if (_zoomType == CZoomType.None) {
            return;
        }

        float amount = STEP_FOW_VALUE * Time.deltaTime;
        if (_zoomType == CZoomType.Out) {
            
            ZoomOut(amount);
            return;
        }
        if (_zoomType == CZoomType.In) {
            
            ZoomIn(amount);
            return;
        }
    }
}

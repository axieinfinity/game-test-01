using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool _isTouched;
    private Vector2 _dragStart;
    private Vector3 _lastCamPos;

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

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Game.Instance.AttackSpeed += 1;
            if (Game.Instance.AttackSpeed > 10)
            {
                Game.Instance.AttackSpeed = 10;
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Game.Instance.AttackSpeed -= 1;
            if (Game.Instance.AttackSpeed < 0)
            {
                Game.Instance.AttackSpeed = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main");
        }

        if (Input.GetMouseButtonDown(0))
        {
            _isTouched = true;
            _dragStart = Input.mousePosition;
            _lastCamPos = gameCamera.transform.localPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isTouched = false;
        }

        if (_isTouched)
        {
            Vector2 currPos = Input.mousePosition;
            Vector2 vDelta = currPos - _dragStart;

            gameCamera.transform.localPosition = _lastCamPos + new Vector3(vDelta.x * 0.01f, vDelta.y * 0.01f, 0);
        }


        if (Input.mouseScrollDelta.magnitude > 0f)
        {
            if (Input.mouseScrollDelta.x > 0)
            {
                ZoomOut(Input.mouseScrollDelta.y);
            } else
            {
                ZoomIn(Input.mouseScrollDelta.y);
            }
            return;
        }

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

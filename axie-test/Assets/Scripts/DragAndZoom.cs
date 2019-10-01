using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndZoom : MonoBehaviour
{
    [SerializeField] Transform miniMapCamera;
    [SerializeField] Transform background;
    Vector3 startPosition;
    Camera mainCamera;
    const float limX = 12.5f;
    const float limY = 8.5f;
    bool allowToDrag = true;
    bool allowToZoom = true;
    Vector3 startBGScale;
    const float minCameraSize = 2f;
    const float startCameraSize = 5f;

    void Start()
    {
        mainCamera = Camera.main;
        startBGScale = background.transform.localScale;
    }

    void Update()
    {
        if (allowToDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                var currentPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var direction = startPosition - currentPosition;
                mainCamera.transform.position += direction;
                miniMapCamera.position += direction;

                CheckOutOfLimit();
            }
            if (Input.GetMouseButtonUp(0))
            {
                allowToDrag = false;
            }
        }
        else
        {
            var pos = mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, Vector3.back * 5f, Time.deltaTime * 10f);
            miniMapCamera.transform.position = Vector3.Lerp(miniMapCamera.transform.position, Vector3.back * 5f, Time.deltaTime * 10f);

            if (Math.Abs(new Vector2(pos.x, pos.y).magnitude) < 0.01f)
            {
                allowToDrag = true;
            }
        }

        if (allowToZoom)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                mainCamera.orthographicSize -= 0.2f;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                mainCamera.orthographicSize += 0.2f;
                var diff = mainCamera.orthographicSize / startCameraSize;
                background.transform.localScale = startBGScale * diff;
            }
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minCameraSize, mainCamera.orthographicSize);
        }
    }

    private void CheckOutOfLimit()
    {
        var pos = mainCamera.transform.position;
        if (Mathf.Abs(pos.x) > limX || Math.Abs(pos.y) > limY)
        {
            allowToDrag = false;
        }
    }
}

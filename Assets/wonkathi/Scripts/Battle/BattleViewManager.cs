using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager drag & zoom camera
/// </summary>
public class BattleViewManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Camera BattleCamera, BackgroundCamera;
    [Range(1, 10)]
    [SerializeField] float moveSpeed = 5;
    [Range(1,10)]
    [SerializeField] float zoomSpeedMouse = 10;
    [Range(1, 10)]
    [SerializeField] float zoomSpeedTouch = 2;

    Vector3 lastMovePosition;

    float minCameraSize;
    float maxCameraSize;
    Vector2 mapSize;
    float minX, minY, maxX, maxY;
    float zoomVel = 0;
    bool wasZoomingLastFrame;
    int moveFingerId;
    Vector2[] lastZoomPositions;
    public float ScaleCamera { get; private set; }
    private void OnEnable()
    {
        minCameraSize = 10;
        maxCameraSize = 0;

        //OrthographicSize relate to height. Fix height to design resolution and calculate the width
        float newWidth = (Screen.width * GameConfig.DesignResolution.y) / Screen.height;
        Vector2 screenUnitSize = new Vector2(newWidth / GameConfig.PixelPerUnit, GameConfig.DesignResolution.y/ GameConfig.PixelPerUnit);
        float bgScale = screenUnitSize.x / background.size.x;
        BackgroundCamera.orthographicSize = GameConfig.InitialCameraSize / bgScale;
    }
    public void UpdateMapSize(Vector2 mapSize)
    {
        this.mapSize = mapSize;
        int mapViewHeightInPixel = 720;
        float mapViewHeightInUnit = mapViewHeightInPixel / GameConfig.PixelPerUnit;
        float scale = mapSize.y / mapViewHeightInUnit;
        maxCameraSize = GameConfig.InitialCameraSize * scale;

        Vector2 halfScreenUnitSize = new Vector2(0.5f * (Screen.width / GameConfig.PixelPerUnit), 0.5f * (Screen.height / GameConfig.PixelPerUnit));

        minX = -mapSize.x / 2f + halfScreenUnitSize.x;
        maxX = mapSize.x / 2f - halfScreenUnitSize.x;
        minY = -mapSize.y / 2f + halfScreenUnitSize.y;
        maxY = mapSize.y / 2f - halfScreenUnitSize.y;
    }

    public void Zoom(bool isZoomIn)
    {
        float newSize = isZoomIn ? minCameraSize : maxCameraSize;
        BattleCamera.orthographicSize = newSize;
        ScaleCamera = newSize / GameConfig.InitialCameraSize;
    }
    private void Update()
    {
        if (Input.touchSupported)
            HandleTouch();
        else HandleMouse();
    }

    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
            lastMovePosition = Input.mousePosition;
        else if (Input.GetMouseButton(0))
        {
            MoveCamera(Input.mousePosition);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, zoomSpeedMouse);   
    }
    void HandleTouch()
    {
        if(Input.touchCount == 1)
        {
            wasZoomingLastFrame = false;
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                lastMovePosition = touch.position;
                moveFingerId = touch.fingerId;
            } else if(touch.fingerId == moveFingerId && touch.phase == TouchPhase.Moved)
            {
                MoveCamera(touch.position);
            }
            return;
        }
        Vector2[] newPositions = new Vector2[2];
        newPositions[0] = Input.GetTouch(0).position;
        newPositions[1] = Input.GetTouch(1).position;
        if (!wasZoomingLastFrame)
        {
            lastZoomPositions = newPositions;
            wasZoomingLastFrame = true;
        } else
        {
            float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
            float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
            float offset = newDistance - oldDistance;
            ZoomCamera(offset, zoomSpeedTouch);
            lastZoomPositions = newPositions;
        }
    }

    void MoveCamera(Vector3 newPosition)
    {
        Vector3 offset = BattleCamera.ScreenToViewportPoint(lastMovePosition - newPosition);
        Vector3 move = offset * moveSpeed* ScaleCamera;
        move.z = 0;
        BattleCamera.transform.Translate(move, Space.World);
        var pos = BattleCamera.transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        BattleCamera.transform.position = pos;
        lastMovePosition = newPosition;
    }

    void ZoomCamera(float offset, float zoomSpeed)
    {
        if (offset == 0 || maxCameraSize == 0)
            return;
        float newSize = BattleCamera.orthographicSize - (offset * zoomSpeed);
        BattleCamera.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        ScaleCamera = newSize / GameConfig.InitialCameraSize;
    }
}

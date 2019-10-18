using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleViewManager : MonoBehaviour
{
    [SerializeField] private Camera BattleCamera;
    [Range(1, 10)]
    [SerializeField] float moveSpeed = 5;
    [Range(1,10)]
    [SerializeField] float zoomSpeed = 10;

    Vector3 lastMovePosition;

    float minCameraSize;
    float maxCameraSize;
    Vector2 mapSize;
    float minX, minY, maxX, maxY;
    public float ScaleCamera { get; private set; }
    private void OnEnable()
    {
        minCameraSize = 10;
        maxCameraSize = 0;
    }
    public void UpdateMapSize(Vector2 mapSize)
    {
        this.mapSize = mapSize;
        int mapViewHeightInPixel = 720;
        float mapViewHeightInUnit = mapViewHeightInPixel / GameConfig.PixelPerUnit;
        float scale = mapSize.y / mapViewHeightInUnit;
        maxCameraSize = GameConfig.InitialCameraSize * scale;

        Vector2 halfScreenUnit = new Vector2(0.5f * (Screen.width / GameConfig.PixelPerUnit), 0.5f * (Screen.height / GameConfig.PixelPerUnit));

        minX = -mapSize.x / 2f + halfScreenUnit.x;
        maxX = mapSize.x / 2f - halfScreenUnit.x;
        minY = -mapSize.y / 2f + halfScreenUnit.y;
        maxY = mapSize.y / 2f - halfScreenUnit.y;
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
        ZoomCamera(scroll);   
    }
    void HandleTouch()
    {

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

    void ZoomCamera(float offset)
    {
        if (offset == 0 || maxCameraSize == 0)
            return;
        float newSize = BattleCamera.orthographicSize - (offset * zoomSpeed);
        BattleCamera.orthographicSize = Mathf.Clamp(newSize, minCameraSize, maxCameraSize);
        ScaleCamera = newSize / GameConfig.InitialCameraSize;
    }
}

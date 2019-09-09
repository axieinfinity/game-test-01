using UnityEngine;

public static class TransformUtils
{
    /// <summary>
    /// Transform a world pos point in another space in to world pos of the point inside this rect transform with same screen position
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="srcCam"></param>
    /// <param name="dstCam"></param>
    /// <param name="worldPos"></param>
    /// <param name="offsetX"></param>
    /// <param name="offsetY"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPosInsideRectTransformWithSameScreenPosition(this RectTransform rectTransform,
        Camera srcCam,
        Camera dstCam,
        Vector3 worldPos,
        float offsetX = 0.0f,
        float offsetY = 0.0f)
    {
        var screenPos = srcCam.WorldToScreenPoint(worldPos) +
                        Vector3.up * offsetY +
                        Vector3.right * offsetX;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform,
            screenPos,
            dstCam,
            out var newPos);
        return rectTransform.GetComponent<Transform>()
            .TransformPoint(newPos);
    }
}
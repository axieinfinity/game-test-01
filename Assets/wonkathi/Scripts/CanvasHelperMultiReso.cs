using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change canvas size base on design resolution
/// </summary>
public class CanvasHelperMultiReso : MonoBehaviour
{
    void Awake()
    {
        var canvasScaler = transform.GetComponent<UnityEngine.UI.CanvasScaler>();
        if (canvasScaler == null)
            return;
        canvasScaler.referenceResolution = GameConfig.DesignResolution;
        var ratio = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
        var physicRatio = Screen.width / (float)Screen.height;
        int match = physicRatio >= ratio ? 1 : 0;
        canvasScaler.matchWidthOrHeight = match;
        transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script calculate the current fps and show it to a text ui.
/// </summary>
public class FPSDisplay : CustomSingleton<FPSDisplay>
{
    public string formatedString = "{value} FPS";
    public Text txtFps;

    public float updateRateSeconds = 4.0F;

    int frameCount = 0;
    float dt = 0.0F;
    float fps = 0.0F;
    bool show;
    float deltaTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            show = !show;
        }
        txtFps.enabled = show;
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        txtFps.text = formatedString.Replace("{value}", System.Math.Round(fps, 1).ToString("0.0"));
    }

    public float GetFps()
    {
        return fps;
    }
}
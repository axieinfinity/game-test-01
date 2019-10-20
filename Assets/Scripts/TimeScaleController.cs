using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleController : MonoBehaviour
{
    public Slider slider;
    
    private void OnEnable()
    {
        slider.value = Time.timeScale;
    }

    public void OnChange()
    {
        Time.timeScale = slider.value;
    }

}

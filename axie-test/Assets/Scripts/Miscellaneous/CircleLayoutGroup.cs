using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLayoutGroup : MonoBehaviour
{
    [SerializeField] float radius;
    [SerializeField] float startAngle;
    [SerializeField] float spacing;
    [SerializeField] bool enableOnAwake;

    private void Awake()
    {
        if (enableOnAwake)
        {
            Enable();
        }
    }

    [ContextMenu("Enable")]
    public void Enable()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var alpha = startAngle + i * spacing;
            transform.GetChild(i).transform.position = transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * alpha) * radius, Mathf.Sin(Mathf.Deg2Rad * alpha) * radius);
        }
    }

    public void Enable(List<GameObject> target)
    {
        int childCount = target.Count;
        for (int i = 0; i < childCount; i++)
        {
            var alpha = startAngle + i * spacing;
            target[i].transform.position = transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * alpha) * radius, Mathf.Sin(Mathf.Deg2Rad * alpha) * radius);
        }
    }

    public void SetValue(float radius = 0, float startAngle = 0, float spacing = 0)
    {
        this.radius = radius;
        this.startAngle = startAngle;
        this.spacing = spacing;
    }
}

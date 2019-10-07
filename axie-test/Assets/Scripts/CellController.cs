using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static GameConstants;

public class CellController : MonoBehaviour
{
    public int circleIndex;
    public Character character;
    public Vector2 gridPosition;
    [SerializeField] SpriteRenderer sprite;

    void Start()
    {

    }

    void Update()
    {

    }

    public void InitValues(float i, float j)
    {
        // var io = i;
        // var mag = Mathf.Sqrt(io * io + j * j);

        // mag = Mathf.RoundToInt(mag);
        // if (mag < 1)
        // {
        //     mag = 1;
        // }
        // circleIndex = (int)mag;
        gridPosition = new Vector2(i, j);
        // i = Mathf.Abs(i);
        // j = Mathf.Abs(j);
        // if (i >= j)
        // {
        //     circleIndex = Mathf.CeilToInt(i + j / 2);
        // }
        // else
        // {
        //     circleIndex = Mathf.RoundToInt(j + (i - 0.5f) / 2);
        // }
        // if (circleIndex >= GridController.instance.width || circleIndex >= GridController.instance.height)
        // {
        //     circleIndex = GridController.instance.width - 1;
        // }
        // if (i == 0 && j == 0)
        // {
        //     circleIndex = 1;
        // }

    }

    public void SetCircleIndex(int value)
    {
        circleIndex = value;
    }

    // public void HighLight()
    // {
    //     sprite.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
    // }
}

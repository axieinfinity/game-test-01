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
        var io = i;
        var mag = Mathf.Sqrt(io * io + j * j);

        mag = Mathf.RoundToInt(mag);
        if (mag < 1)
        {
            mag = 1;
        }
        circleIndex = (int)mag;

        // var t = i + offset;
        // t = t < 0 ? Mathf.FloorToInt(t) : Mathf.CeilToInt(t);
        gridPosition = new Vector2(i, j);

        // this.Log("i: " + i + ".j: " + j);
        // circleIndex = (new Vector2Int(i, j).magnitude);
    }

    public void HighLight()
    {
        sprite.DOColor(Color.red, 0.5f).SetLoops(2, LoopType.Yoyo);
    }
}

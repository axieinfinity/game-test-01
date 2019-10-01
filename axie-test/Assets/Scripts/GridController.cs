using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CharacterModel;
using static GameConstants;

public class GridController : CustomSingleton<GridController>
{
    [SerializeField] CellController prefab;
    [SerializeField] int width, height;
    Dictionary<Vector2, CellController> cellsDictionary;

    public override void Awake()
    {
        cellsDictionary = new Dictionary<Vector2, CellController>();
        base.Awake();
    }

    public CellController GetCell(Vector2 position)
    {
        return cellsDictionary[position];
    }

    void Start()
    {
        CreateGrid();
    }

    public List<CellController> GetPointByCircleIndex(int circleIndex)
    {
        if (circleIndex > (width > height ? width : height))
        {
            return null;
        }
        var result = new List<CellController>();
        foreach (var item in cellsDictionary)
        {
            var ele = item.Value;
            if (ele.circleIndex.Equals(circleIndex))
            {
                result.Add(ele);
            }
        }
        return result;
    }

    public void RemoveCharacterFromCell(Vector2 gridPosition)
    {
        cellsDictionary[gridPosition].character = null;
    }

    private void CreateGrid()
    {
        var j = 0;
        var k = 0;
        var offset = 0f;
        while (j <= height)
        {
            for (int i = -width + k; i <= width; i++)
            {
                offset = k * -0.5f;
                var obj = default(CellController);
                if (j > 0)
                {
                    for (int p = 0; p < 2; p++)
                    {
                        var mul = p == 0 ? 1 : -1;
                        obj = Instantiate<CellController>(prefab, transform);
                        obj.transform.transform.localPosition = new Vector3(i + offset, j * mul);
                        obj.InitValues(i + offset, j * mul);

                        // var t = i + offset;
                        // t = t < 0 ? Mathf.FloorToInt(t) : Mathf.CeilToInt(t);
                        obj.name += ".[" + (i + offset) + "." + (j * mul) + "]";
                        cellsDictionary.Add(new Vector2((i + offset), j * mul), obj);
                    }
                }
                else
                {
                    obj = Instantiate<CellController>(prefab, transform);
                    obj.transform.localPosition = new Vector3(i, j, 0);
                    obj.InitValues(i, j);
                    obj.name += ".[" + i + "." + (j) + "]";
                    cellsDictionary.Add(new Vector2(i, j), obj);
                }
            }
            k++;
            j++;
        }
        this.PostEvent(EventID.ON_GRID_HAS_INIT);
    }

    public Character GetAdjacentEnemy(Vector2 gPos, CHARACTER_TYPE type)
    {
        // this.Log("finder pos: " + gPos);
        var adjacentCells = GetAdjacentCells(gPos);
        for (int i = 0; i < adjacentCells.Count; i++)
        {
            var ele = adjacentCells[i].character;
            if (ele != null && ele.model.type == type)
            {
                return ele;
            }
        }
        return null;
    }

    public CellController GetAdjacentPosition(Vector2 gPos)
    {
        var adjacentCells = GetAdjacentCells(gPos);
        for (int i = 0; i < adjacentCells.Count; i++)
        {
            var ele = adjacentCells[i];
            if (ele.character == null)
            {
                return ele;
            }
        }
        return null;
    }

    public List<CellController> GetAdjacentCells(Vector2 gPos)
    {
        var result = new List<CellController>();
        Vector2[] arr = {
            new Vector2(gPos.x - 1, gPos.y),
            new Vector2(gPos.x - 0.5f, gPos.y + 1),
            new Vector2(gPos.x + 0.5f, gPos.y + 1),
            new Vector2(gPos.x + 1, gPos.y),
            new Vector2(gPos.x + 0.5f , gPos.y - 1),
            new Vector2(gPos.x - 0.5f, gPos.y - 1),
        };
        for (int i = 0; i < arr.Length; i++)
        {
            var ele = arr[i];
            if (cellsDictionary.ContainsKey(ele) == false)
            {
                continue;
            }
            else
            {
                result.Add(cellsDictionary[ele]);
            }
        }
        return result;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HexaGrid
/// </summary>

public class HexaGrid : MonoBehaviour
{
    [SerializeField] float size;
    [SerializeField] HexaUnit unitPrefab;
    [SerializeField] int radius;

    private List<HexaUnit> _unitList;
    private Vector2 _center;

    private readonly Vector2Int[] _DIRECTION = {
        new Vector2Int(2, 0),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-2, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(1, 1)
        
    };

    //for pooling
    //check from pool first
    private HexaUnit GetHexaUnit()
    {
        HexaUnit unit;

        if (_unitList == null)
        {
            unit = Instantiate(unitPrefab, transform);
            unit.gameObject.SetActive(true);
            _unitList.Add(unit);

            return unit;
        }

        if (_unitList.Count == 0)
        {
            unit = Instantiate(unitPrefab, transform);
            unit.gameObject.SetActive(true);
            _unitList.Add(unit);

            return unit;
        }

        foreach (var item in _unitList)
        {
            if (!item.gameObject.activeSelf)
            {
                unit = item;
                unit.gameObject.SetActive(true);
                return unit;
            }
        }

        unit = Instantiate(unitPrefab, transform);
        unit.gameObject.SetActive(true);
        _unitList.Add(unit);

        return unit;

    }

    //temporary deactive, re-active when needed
    private void ReleaseUnit(HexaUnit unit)
    {
        unit.gameObject.SetActive(false);
    }

    //generate Hexa units by given Radius and center,
    //layout them by Hexa Unit image size base
    public void GenerateUnits ()
    {
        if (unitPrefab == null)
        {
            Debug.LogError("Please assign HexaUnit as Prefab first.");
            return;
        }

		//for sure, if null, allocate first
		if (_unitList == null)
        {
            _unitList = new List<HexaUnit>();
        } else
        {
            if (_unitList.Count > 0)
            {
                //clear all before create new ones
                foreach (var item in _unitList)
                {
                    //because objects are deactived sequency,
                    //break if found first deactived one
                    if (!item.gameObject.activeSelf)
                    {
                        break;
                    }
                    ReleaseUnit(item);
                }
            }
        }

        //get center from grid base position
        _center = transform.localPosition;

        //pre-calculate hexaunit's size
        float sizeW = Mathf.Sqrt(3f) * size; 
        float sizeH = 2 * size;


        if (radius == 0)
        {
            HexaUnit unit = GetHexaUnit();
            unit.Index = Vector2Int.zero;
            unit.UpdateInfo();
            unit.transform.localPosition = _center;

            return;
        }

        for (int i = -radius; i <= radius; ++i)
        {
            for (int j = -radius * 2; j <= radius * 2; ++j)
            {
                if ((Mathf.Abs(i - j) % 2 != 0))
                {
                    //do nothing
                    //cause: not available on grid system
                }
                else if (Mathf.Abs(i) + Mathf.Abs(j) > radius * 2)
                {
                    //do nothing
                    //cause: outside the radius
                }
                else
                {
                    HexaUnit unit = GetHexaUnit();
                    unit.Index = new Vector2Int(i, j);
                    unit.UpdateInfo();

                    float pX = _center.x + j * sizeW * 0.5f; 
                    float pY = _center.y + i * sizeH * 0.75f;

                    unit.transform.localPosition = new Vector2(pX, pY);
                }
            }
        }
    }
}
 
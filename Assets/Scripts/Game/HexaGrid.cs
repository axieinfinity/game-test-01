using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HexaGrid
/// </summary>
[ExecuteInEditMode]
public class HexaGrid : MonoBehaviour
{
    [Header("Grid Config")]
    [SerializeField] float size;
    [SerializeField] int radius;

    [Header("Default Config")]
    [SerializeField] List<Character> characterPrefabs;
    [SerializeField] HexaUnit unitPrefab;

    public int Radius
    {
        get => radius;
        set => radius = value;
    }

    private List<HexaUnit> _unitList;
    private List<Character> _characterList;
    private Vector2 _center;

    private readonly Vector2Int[] _DIRECTION = {
        new Vector2Int(2, 0),
        new Vector2Int(1, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-2, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(1, 1)
        
    };

    //for pooing
    //checkCharacters from pool first
    private Character GetCharacter (Character.CType type)
    {
        Character character;

        if (_characterList == null)
        {
            character = Instantiate(characterPrefabs[(int)type], transform);
            character.gameObject.SetActive(true);
            _characterList.Add(character);

            return character;
        }

        if (_characterList.Count == 0)
        {
            character = Instantiate(characterPrefabs[(int)type], transform);
            character.gameObject.SetActive(true);
            _characterList.Add(character);

            return character;
        }

        foreach (var item in _characterList)
        {
            if (!item.gameObject.activeSelf && item.Type == type)
            {
                character = item;
                character.gameObject.SetActive(true);

                return character;
            }
        }

        character = Instantiate(characterPrefabs[(int)type], transform);
        character.gameObject.SetActive(true);
        _characterList.Add(character);

        return character;
    }

    //temporary deactive, re-active when needed
    private void ReleaseCharacter(Character character)
    {
        character.gameObject.SetActive(false);
    }

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

    [ContextMenu("Clear")]
    //clear Hexagrid and destroy all units
    public void Clear ()
    {
        if (_unitList != null && _unitList.Count > 0)
        {
            //clear all before create new ones
            foreach (var item in _unitList)
            {
                DestroyImmediate(item.gameObject, false);
            }

            _unitList.Clear();
        }

        if (_characterList != null && _characterList.Count > 0)
        {
            //clear all before create new ones
            foreach (var item in _characterList)
            {
                DestroyImmediate(item.gameObject, false);
            }

            _characterList.Clear();
        }
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

        if (characterPrefabs == null)
        {
            Debug.LogError("Please assign Characters as Prefab first.");
            return;
        }

        if (characterPrefabs.Count == 0)
        {
            Debug.LogError("Please assign Characters as Prefab first.");
            return;
        }

        //for sure, if null, allocate first
        if (_characterList == null)
        {
            _characterList = new List<Character>();
        }
        else
        {
            if (_characterList.Count > 0)
            {
                //clear all before create new ones
                foreach (var item in _characterList)
                {
                    ReleaseCharacter(item);
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

            //only one unit, spawn an Defender
            Character character = GetCharacter(Character.CType.Defender);
            character.HexaUnit = unit;
            character.UpdatePosition();

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
                    Vector2 uCenter = new Vector2(pX, pY);
                    unit.transform.localPosition = uCenter;
                    float d = Vector2.Distance(uCenter, _center);
                    int rIdx = (int)(d / (sizeH * 0.75f));

                    if (rIdx < (radius + 1) / 2)
                    {
                        //means Defender should be generated
                        Character character = GetCharacter(Character.CType.Defender);
                        character.HexaUnit = unit;
                        character.UpdatePosition();
                    } else if (rIdx == (radius + 1) / 2)
                    {
                        //mean center line, don't do anything
                        
                    } else
                    {
                        //mean outside circles, Attacker shoudl be generated
                        Character character = GetCharacter(Character.CType.Attacker);
                        character.HexaUnit = unit;
                        character.UpdatePosition();
                    }
                }
            }
        }
    }
}
 
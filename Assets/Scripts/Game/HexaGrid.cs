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

    private List<HexaUnit> _poolUnitList;
    private List<Character> _poolCharacterList;
    private Vector2 _center;
    private List<HexaUnit> _unitList;
    private List<Character> _characterList;
    private List<Character> _movableList;

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

        if (_poolCharacterList == null)
        {
            character = Instantiate(characterPrefabs[(int)type], transform);
            character.gameObject.SetActive(true);
            _poolCharacterList.Add(character);

            return character;
        }

        if (_poolCharacterList.Count == 0)
        {
            character = Instantiate(characterPrefabs[(int)type], transform);
            character.gameObject.SetActive(true);
            _poolCharacterList.Add(character);

            return character;
        }

        foreach (var item in _poolCharacterList)
        {
            if (!item.gameObject.activeSelf && item.Type == type)
            {
                character = item;
                character.gameObject.SetActive(true);
                character.ResetStats();
                return character;
            }
        }

        character = Instantiate(characterPrefabs[(int)type], transform);
        character.gameObject.SetActive(true);
        _poolCharacterList.Add(character);

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

        if (_poolUnitList == null)
        {
            unit = Instantiate(unitPrefab, transform);
            unit.gameObject.SetActive(true);
            _poolUnitList.Add(unit);

            return unit;
        }

        if (_poolUnitList.Count == 0)
        {
            unit = Instantiate(unitPrefab, transform);
            unit.gameObject.SetActive(true);
            _poolUnitList.Add(unit);

            return unit;
        }

        foreach (var item in _poolUnitList)
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
        _poolUnitList.Add(unit);

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
        if (_poolUnitList != null && _poolUnitList.Count > 0)
        {
            //clear all before create new ones
            foreach (var item in _poolUnitList)
            {
                DestroyImmediate(item.gameObject, false);
            }

            _poolUnitList.Clear();
        }

        if (_poolCharacterList != null && _poolCharacterList.Count > 0)
        {
            //clear all before create new ones
            foreach (var item in _poolCharacterList)
            {
                DestroyImmediate(item.gameObject, false);
            }

            _poolCharacterList.Clear();
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
		if (_poolUnitList == null)
        {
            _poolUnitList = new List<HexaUnit>();
        } else
        {
            if (_poolUnitList.Count > 0)
            {
                //clear all before create new ones
                foreach (var item in _poolUnitList)
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
        if (_poolCharacterList == null)
        {
            _poolCharacterList = new List<Character>();
        }
        else
        {
            if (_poolCharacterList.Count > 0)
            {
                //clear all before create new ones
                foreach (var item in _poolCharacterList)
                {
                    ReleaseCharacter(item);
                }
            }
        }

        //get center from grid base position
        _center = transform.localPosition;

        //clear Old lists
        if (_unitList == null) _unitList = new List<HexaUnit>();
        _unitList.Clear();

        if (_characterList == null) _characterList = new List<Character>();
        _characterList.Clear();

        //pre-calculate hexaunit's size
        float sizeW = Mathf.Sqrt(3f) * size; 
        float sizeH = 2 * size;


        if (radius == 0)
        {
            HexaUnit unit = GetHexaUnit();
            unit.Index = Vector2Int.zero;
            unit.UpdateInfo();
            unit.transform.localPosition = _center;
            _unitList.Add(unit);
            //only one unit, spawn an Defender
            Character character = GetCharacter(Character.CType.Defender);
            character.HexaUnit = unit;
            unit.Character = character;
            character.UpdatePosition();
            _characterList.Add(character);

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
                    _unitList.Add(unit);
                    float d = Vector2.Distance(uCenter, _center);
                    int rIdx = (int)(d / (sizeH * 0.75f));

                    if (rIdx < (radius + 1) / 2)
                    {
                        //means Defender should be generated
                        Character character = GetCharacter(Character.CType.Defender);
                        character.HexaUnit = unit;
                        unit.Character = character;
                        character.UpdatePosition();
                        _characterList.Add(character);
                    } else if (rIdx == (radius + 1) / 2)
                    {
                        //mean center line, don't assign any character at the begining
                        
                    } else
                    {
                        //mean outside circles, Attacker shoudl be generated
                        Character character = GetCharacter(Character.CType.Attacker);
                        character.HexaUnit = unit;
                        unit.Character = character;
                        character.UpdatePosition();
                        _characterList.Add(character);
                    }
                }
            }
        }
    }

    public void MoveToAroundCharacter(Character attacker)
    {
        if (_movableList != null)
        {
            if (!_movableList.Contains(attacker))
            {
                return;
            }
        }

        for (int i = 0; i < _DIRECTION.Length; ++i)
        {
            int iIdx = attacker.HexaUnit.Index.x + _DIRECTION[i].y;
            int jIdx = attacker.HexaUnit.Index.y + _DIRECTION[i].x;

            Character character = FindCharacterByHexaIndex(new Vector2Int(iIdx, jIdx));
            if (character == null)
            {
                continue;
            }

            if (character.Type != Character.CType.Defender)
            {
                continue;
            }

            //found first nearleast one, attack!
            //attacker.Attack(character);
            return;
        }


        //TODO: fix (optimize) when having time
        //should find nearlest, temporary by time rush
        float minDistance = float.MaxValue;
        HexaUnit target = null;

        foreach (Character item in _characterList)
        {
            if (item.Type == Character.CType.Attacker)
            {
                continue;
            }

            if (item.IsDead)
            {
                continue;
            }

            float d = Vector2.Distance(item.transform.localPosition, attacker.transform.localPosition);
            if (d < minDistance)
            {
                HexaUnit unit = FindEmptyPosition(item);
                if (unit != null)
                {
                    minDistance = d;
                    target = unit;
                }
            }
        }

        if (target != null)
            attacker.Move(target);
    }

    public void ActionToAroundCharacter(Character attacker)
    {
        //find around neighbours first
        for (int i = 0; i < _DIRECTION.Length; ++i)
        {
            int iIdx = attacker.HexaUnit.Index.x + _DIRECTION[i].y;
            int jIdx = attacker.HexaUnit.Index.y + _DIRECTION[i].x;

            Character character = FindCharacterByHexaIndex(new Vector2Int(iIdx, jIdx));
            if (character == null)
            {
                continue;
            }

            if (character.Type != Character.CType.Defender)
            {
                continue;
            }

            //found first nearleast one, attack!
            attacker.Attack(character);
            return;
        }        
    }

    private HexaUnit FindEmptyPosition(Character target)
    {
        HexaUnit unit = null;
        for (int i = 0; i < _DIRECTION.Length; ++i)
        {
            int iIdx = target.HexaUnit.Index.x + _DIRECTION[i].y;
            int jIdx = target.HexaUnit.Index.y + _DIRECTION[i].x;

            Character character = FindCharacterByHexaIndex(new Vector2Int(iIdx, jIdx));
            if (character == null)
            {
                unit = FindHexaUnitByHexaIndex(new Vector2Int(iIdx, jIdx));

                if (unit != null)
                    break;
            }
        }

        return unit;
    }

    private HexaUnit FindHexaUnitByHexaIndex(Vector2Int hIdx)
    {
        //TODO: Fix (optimize) when having time
        HexaUnit unit = null;

        foreach (HexaUnit item in _unitList)
        {
            if (item.Index.Equals(hIdx))
            {
                unit = item;
                break;
            }
        }

        return unit;
    }

    private Character FindCharacterByHexaIndex(Vector2Int hIdx)
    {
        //TODO: Fix (optimize) when having time
        Character character = null;

        foreach (Character item in _characterList)
        {
            if (item.HexaUnit.Index.Equals(hIdx))
            {
                character = item;
                break;
            }
        }

        return character;
    }

    public void RemoveCharacter(Character character)
    {
        ReleaseCharacter(character);
        _characterList.Remove(character);
        _movableList.Remove(character);
    }

    public void FindAllMovableCharacters()
    {
        if (_movableList == null) _movableList = new List<Character>();
        _movableList.Clear();

        foreach (Character item  in _characterList)
        {
            if (item.Type == Character.CType.Defender)
            {
                continue;
            }

            if (FindEmptyPosition(item) != null)
            {
                _movableList.Add(item);
            }
        }
    }
}
 
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CircleUnit : MonoBehaviour
{
    SpriteRenderer Sprite;
    public DTCircleUnit Data { get; private set; }
    public Character Character { get; private set; }
    public Character BookedCharacter { get; private set; }

    private List<CircleUnit> adjacents = new List<CircleUnit>();
    public ReadOnlyCollection<CircleUnit> Adjacents { get; private set; }
    private void OnEnable()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }
    public void Init(DTCircleUnit dTCircleUnit)
    {
        transform.localPosition = dTCircleUnit.BasePosition;
        Data = dTCircleUnit;
    }
    public void UpdateAdjacents(List<CircleUnit> units)
    {
        adjacents.Clear();
        adjacents.AddRange(units);
        Adjacents = adjacents.AsReadOnly();
    }
    public void UpdateCharacter(Character character)
    {
        Character = character;
        BookedCharacter = null;
    }
    public void CharacterBook(Character character)
    {
        BookedCharacter = character;
    }
}

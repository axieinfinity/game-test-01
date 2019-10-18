using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapDot : MonoBehaviour
{
    [SerializeField] Color attackerColor = Color.cyan;
    [SerializeField] Color defensorColor = Color.green;
    [SerializeField] SpriteRenderer sprite;
    public Character Character { get; private set; }
    public void SetCharacter(Character character)
    {
        Character = character;
        sprite.color = character.Data.Type == EnCharacterType.Attacker ? attackerColor : defensorColor;
    }
}

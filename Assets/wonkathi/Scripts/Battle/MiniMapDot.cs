using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapDot : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    public Character Character { get; private set; }
    public void SetCharacter(Character character)
    {
        Character = character;
        sprite.color = character.Data.Type == EnCharacterType.Attacker ? Color.cyan : Color.green;
    }
}

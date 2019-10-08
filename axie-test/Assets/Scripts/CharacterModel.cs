using UnityEngine;

[CreateAssetMenu(fileName = "CharacterModel", menuName = "axie-test/CharacterModel", order = 0)]
public class CharacterModel : ScriptableObject
{
    
    public enum CHARACTER_TYPE
    {
        ATTACK, DEFENSE
    }
    public CHARACTER_TYPE type;
    public bool movable;
    public float startHP;
}
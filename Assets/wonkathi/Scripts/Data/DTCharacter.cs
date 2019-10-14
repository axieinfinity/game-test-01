using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnCharacterType
{
    Defensor,
    Attacker
}
public class DTCharacter
{
    public int BaseHP { get; private set; }
    public int CurrentHP;
    public EnCharacterType Type { get; private set; }
    public DTCharacter() { }
    public DTCharacter(Dictionary<string, object> data) { this.ParseData(data); }
    public void ParseData(Dictionary<string, object> data)
    {

    }
    public DTCharacter(int baseHP, EnCharacterType type)
    {
        BaseHP = baseHP;
        Type = type;
        CurrentHP = baseHP;
    }
}

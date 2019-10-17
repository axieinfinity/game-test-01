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

    int lastHP;
    private int currentHP;
    public int CurrentHP
    {
        get
        {
            return currentHP;
        }
        set
        {
            lastHP = currentHP;
            currentHP = value;
        }
    }

    public bool IsHPChanged
    {
        get
        {
            return !currentHP.Equals(lastHP);
        }
    }
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
        lastHP = CurrentHP;
    }
}

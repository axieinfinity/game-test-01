using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will proceed defensor's behaviour
/// </summary>
public class Defensor : Character
{
    public override bool CheckAction()
    {
        base.CheckAction();
        var adjacents = StandingBase.Adjacents;
        foreach (var a in adjacents)
        {
            if (a.BookedCharacter != null)
                continue;
            if (a.Character != null && a.Character.Data.Type == EnCharacterType.Attacker)
            {
                Action.SetAction(EnCharacterAction.Attack, a.Character);
                return true;
            }
        }
        return false;
    }
}

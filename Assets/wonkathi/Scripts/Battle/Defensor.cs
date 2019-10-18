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
        Character target = null;
        float targetHP = GameConfig.AttackerBaseHP + 1;
        foreach (var a in adjacents)
        {
            if (a.Character != null && a.Character.Data.Type == EnCharacterType.Attacker && targetHP > a.Character.Data.CurrentHP)
            {
                target = a.Character;
                targetHP = target.Data.CurrentHP;
                continue;
            }
        }
        if(target != null)
        {
            Action.SetAction(EnCharacterAction.Attack, target);
            return true;
        }
        return false;
    }
}

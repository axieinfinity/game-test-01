using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will proceed attacker's behaviour
/// </summary>
public class Attacker : Character
{
    CircleUnit moveToTarget;
    float speed;
    public override bool CheckAction()
    {
        base.CheckAction();
        if (ClosestEnemy == null)
            return true;
        SwitchBase();
        var adjacents = StandingBase.Adjacents;
        if(adjacents.Contains(ClosestEnemy.StandingBase))
        {
            Action.SetAction(EnCharacterAction.Attack, ClosestEnemy);
            return true;
        }
        float minDistance = Vector2.Distance(StandingBase.Data.BasePosition, ClosestEnemy.StandingBase.Data.BasePosition);
        CircleUnit target = null;
        foreach(var a in adjacents)
        {
            if (a.BookedCharacter != null)
                continue;
            if(a.Character != null && a.Character.Data.Type == EnCharacterType.Defensor)
            {
                Action.SetAction(EnCharacterAction.Attack, a.Character);
                return true;
            }
            if (CanMoveTo(a, ref minDistance))
                target = a;
        }
        if (target != null)
        {
            target.CharacterBook(this);
            Action.SetAction(EnCharacterAction.Move, target);
            return true;
        }
        return false;
    }

    bool CanMoveTo(CircleUnit unit, ref float minDistance)
    {
        if (unit.Character != null /*|| unit.Data.Round >= StandingBase.Data.Round*/)
            return false;
        var distance = Vector2.Distance(unit.Data.BasePosition, ClosestEnemy.StandingBase.Data.BasePosition);
        if (distance < minDistance)
        {
            minDistance = distance;
            return true;
        }
        return false;
    }
    private void Update()
    {
        if (moveToTarget != null)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, moveToTarget.transform.position + new Vector3(0, -0.7f, 0), step);
            if(Vector3.Distance(transform.position, moveToTarget.transform.position) < 0.001f)
            {
                SwitchBase();
            }
        }
    }

    void SwitchBase()
    {
        if (moveToTarget == null)
            return;
        StandingBase.UpdateCharacter(null);
        moveToTarget.UpdateCharacter(this);
        UpdateStandingBase(moveToTarget);
        moveToTarget = null;
    }

    public override void MoveTo(CircleUnit unit)
    {
        base.MoveTo(unit);
        speed = GameConfig.CircleUnitRadius * 2f / (0.9f * GameConfig.CharacterActionDuration);
        moveToTarget = unit;
    }
}

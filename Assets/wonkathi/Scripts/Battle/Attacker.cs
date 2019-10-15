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
    public override void CheckAction()
    {
        base.CheckAction();
        SwitchBase();
        var adjacents = StandingBase.Adjacents;
        moveToTarget = null;
        float minDistance = 1000;
        foreach(var a in adjacents)
        {
            if (a.BookedCharacter != null)
                continue;
            if(a.Character != null && a.Character.Data.Type == EnCharacterType.Defensor)
            {
                Attack(a.Character);
                return;
            }
            if (CanMoveTo(a, ref minDistance))
                moveToTarget = a;
        }
        if (moveToTarget != null)
        {
            moveToTarget.CharacterBook(this);
            MoveTo(moveToTarget);
        }
        speed = GameConfig.CircleUnitRadius * 2f / (0.9f*GameConfig.CharacterActionDuration);
    }

    bool CanMoveTo(CircleUnit unit, ref float minDistance)
    {
        if (unit.Character != null || unit.Data.Round >= StandingBase.Data.Round)
            return false;
        var adjacents = unit.Adjacents;
        bool canMove = false;
        foreach(var a in adjacents)
        {
            if(a.Data.Round < StandingBase.Data.Round)
            {
                var distance = Vector2.Distance(a.Data.BasePosition, StandingBase.Data.BasePosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    canMove = true;
                }
            }
        }
        return canMove;
    }
    private void Update()
    {
        if(moveToTarget != null)
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
}

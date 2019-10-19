using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will proceed attacker's behaviour
/// </summary>
public class Attacker : Character
{
    /// <summary>
    /// Next unit this character will move to
    /// </summary>
    CircleUnit moveToTarget;
    /// <summary>
    /// When the character attack, he will move to enemy and back. This the the path of movement
    /// </summary>
    List<Vector3> pathTargets = new List<Vector3>();
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
        CircleUnit targetMove = null;
        Character targetAtk = null;
        float targetAtkHP = GameConfig.DefensorBaseHP + 1;
        foreach(var a in adjacents)
        {
            //Find nearest enemy to attack. If there is more than 1 enemy, the character will attack the one which lower hp
            if(a.Character != null && a.Character.Data.Type == EnCharacterType.Defensor && targetAtkHP > a.Character.Data.CurrentHP)
            {
                targetAtk = a.Character;
                targetAtkHP = targetAtk.Data.CurrentHP;
                continue;
            }
            if (CanMoveTo(a, ref minDistance))
                targetMove = a;
        }
        if(targetAtk != null)
        {
            Action.SetAction(EnCharacterAction.Attack, targetAtk);
            return true;
        }
        if (targetMove != null)
        {
            targetMove.CharacterBook(this);
            Action.SetAction(EnCharacterAction.Move, targetMove);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check and find the nearest empty circle unit that the character can move to.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="minDistance"></param>
    /// <returns></returns>
    bool CanMoveTo(CircleUnit unit, ref float minDistance)
    {
        if (unit.Character != null  || unit.BookedCharacter != null)
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
        // Check attack movement
        if (pathTargets.Count > 0)
        {
            float step = moveSpeed * Time.deltaTime;
            if (pathTargets.Count == 1)
                step *= 1.5f;
            else step *= 0.5f;
            transform.position = Vector3.MoveTowards(transform.position, pathTargets[0], step);
            if (Vector3.Distance(transform.position, pathTargets[0]) < 0.001f)
            {
                pathTargets.RemoveAt(0);
            }
            return;
        }
        //Move to next empty circle
        if (moveToTarget != null)
        {
            float step = moveSpeed * Time.deltaTime;
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
        moveSpeed = GameConfig.CircleUnitRadius * 2f / (0.9f * GameConfig.CharacterActionDuration);
        moveToTarget = unit;
    }
    public override void Attack(Character target)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.Attack2, false);
        base.Attack(target);

        pathTargets.Clear();
        var desire = target.transform.position - transform.position;
        pathTargets.Add(transform.position + desire.normalized * GameConfig.CircleUnitRadius);
        pathTargets.Add(transform.position);
        var distance = desire.magnitude;
        moveSpeed =(distance / 0.5f);
    }
    public override void Die()
    {
        base.Die();
        var fx = EffectManager.Inst.GetEffect(EffectName.AttackerDie);
        if (fx != null)
        {
            fx.gameObject.SetActive(true);
            fx.layer = LayerMask.NameToLayer(LayerName.Battle);
            fx.transform.position = StandingBase.transform.position;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will proceed defensor's behaviour
/// </summary>
public class Defensor : Character
{ 
    /// <summary>
    /// When the character attack, he will move to enemy and back. This the the path of movement
    /// </summary>
    List<Vector3> pathTargets = new List<Vector3>();
    public override bool CheckAction()
    {
        base.CheckAction();
        var adjacents = StandingBase.Adjacents;
        Character target = null;
        float targetHP = GameConfig.AttackerBaseHP + 1;
        foreach (var a in adjacents)
        {
            //Find nearest enemy to attack. If there is more than 1 enemy, the character will attack the one which lower hp
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

    public override void Attack(Character target)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.Attack1, false);
        base.Attack(target);

        pathTargets.Clear();
        var desire = target.transform.position - transform.position;
        pathTargets.Add(transform.position + desire.normalized * GameConfig.CircleUnitRadius);
        pathTargets.Add(transform.position);
        var distance = desire.magnitude;
        moveSpeed = (distance / 0.5f);
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
        }
    }

    public override void Die()
    {
        base.Die();
        var fx = EffectManager.Inst.GetEffect(EffectName.DefensorDie);
        if (fx != null)
        {
            fx.gameObject.SetActive(true);
            fx.layer = LayerMask.NameToLayer(LayerName.Battle);
            fx.transform.position = StandingBase.transform.position;
        }
    }
}

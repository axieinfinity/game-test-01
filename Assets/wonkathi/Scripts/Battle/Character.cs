using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will proceed character's behaviour
/// </summary>
/// 
public enum EnCharacterAction
{
    None,
    Move,
    Attack,
    Die
}
public class CharacterAction
{
    public Character TargetCharacter { get; private set; }
    public CircleUnit TargetCC { get; private set; }
    public EnCharacterAction Action { get; private set; }
    public void SetAction(EnCharacterAction action, Character targetCharacter, CircleUnit targetCC)
    {
        this.Action = action;
        this.TargetCharacter = targetCharacter;
        this.TargetCC = targetCC;
    }

    public void SetAction(EnCharacterAction action, Character targetCharacter)
    {
        SetAction(action, targetCharacter, null);
    }

    public void SetAction(EnCharacterAction action, CircleUnit targetCC)
    {
        SetAction(action, null, targetCC);
    }

    public void Reset()
    {
        this.Action = EnCharacterAction.None;
        TargetCharacter = null;
        TargetCC = null;
    }
}
public class Character : MonoBehaviour
{
    public Character ClosestEnemy { get; protected set; }
    public int AttackValue { get; private set; }
    SkeletonAnimation skeletonAnimation;
    MeshRenderer meshRender;
    public Vector2 Size { get; private set; }
    public DTCharacter Data { get; private set; }
    public CircleUnit StandingBase { get; private set; }

    protected CharacterAction Action = new CharacterAction();
    public System.Action<Character> OnCharacterDie;
    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        meshRender = GetComponent<MeshRenderer>();
        Size = meshRender.bounds.size;
    }

    public virtual void SetData(DTCharacter data)
    {
        this.Data = data;
        skeletonAnimation.AnimationState.AddAnimation(0, AnimationAction.Idle, true, 0);
    }
    public virtual void UpdateClosestEnemy(Character character)
    {
        ClosestEnemy = character;
    }

    public virtual bool CheckAction()
    {
        Action.Reset();
        AttackValue = Random.Range(0, 3);
        return true;
    }
    public virtual void DoAction()
    {
        switch (Action.Action)
        {
            case EnCharacterAction.Move:
                {
                    MoveTo(Action.TargetCC);
                    break;
                }
            case EnCharacterAction.Attack:
                {
                    int attackLogicValue = (3 + AttackValue - Action.TargetCharacter.AttackValue)%3;
                    int damage = attackLogicValue == 0 ? 4 : attackLogicValue == 1 ? 5 : 3;
                    Attack(Action.TargetCharacter);
                    Action.TargetCharacter.GotHit(this, damage);
                    break;
                }
            case EnCharacterAction.Die:
                {
                    Die();
                    break;
                }
            default:
                break;
        }
    }
    public void UpdateStandingBase(CircleUnit unit)
    {
        StandingBase = unit;
    }
    public virtual void Attack(Character target)
    {
        bool isFlipX = target.transform.position.x > transform.position.x;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFlipX ? -1 : 1);
        transform.localScale = scale;
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.Attack, false);
        skeletonAnimation.AnimationState.AddAnimation(0, AnimationAction.MoveBack, false, 0);
        skeletonAnimation.AnimationState.AddAnimation(0, AnimationAction.Idle, true, 0);
    }
    public virtual void GotHit(Character enemy, int damage)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.GotHit, false);
        skeletonAnimation.AnimationState.AddAnimation(0, AnimationAction.Idle, true, 0);
        Data.CurrentHP -= damage;
        if (Data.CurrentHP <= 0)
            Die();
    }

    public virtual void MoveTo(CircleUnit unit)
    {
        bool isFlipX = unit.Data.BasePosition.x > StandingBase.Data.BasePosition.x;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFlipX ? -1 : 1);
        transform.localScale = scale;
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.MoveBack, false);
        skeletonAnimation.AnimationState.AddAnimation(1, AnimationAction.Idle, true, 0);
    }
    public void Die()
    {
        StandingBase.UpdateCharacter(null);
        if (OnCharacterDie != null)
            OnCharacterDie.Invoke(this);
    }
}

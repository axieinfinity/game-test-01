using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will proceed character's behaviour
/// </summary>
public class Character : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    MeshRenderer meshRender;
    public Vector2 Size { get; private set; }
    public DTCharacter Data { get; private set; }

    public CircleUnit StandingBase { get; private set; }

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

    public virtual void CheckAction()
    {

    }
    public void UpdateStandingBase(CircleUnit unit)
    {
        StandingBase = unit;
    }
    public virtual void Attack(Character target)
    {
        target.GotHit(this);
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.Attack, false);
        skeletonAnimation.AnimationState.AddAnimation(0, AnimationAction.MoveBack, false, 0);
        skeletonAnimation.AnimationState.AddAnimation(0, AnimationAction.Idle, true, 0);
    }
    public virtual void GotHit(Character enemy)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.GotHit, false);
        skeletonAnimation.AnimationState.AddAnimation(0, AnimationAction.Idle, true, 0);
    }

    public void MoveTo(CircleUnit unit)
    {
        skeletonAnimation.AnimationState.SetAnimation(0, AnimationAction.MoveBack, false);
        skeletonAnimation.AnimationState.AddAnimation(1, AnimationAction.Idle, true, 0);
    }
}

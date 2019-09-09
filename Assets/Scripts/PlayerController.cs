using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnimation;

    public Transform EnemyTransform;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        //skeletonAnimation.Skeleton.FlipX = true;
        ////backPos = transform.position;



        //skeletonAnimation.AnimationState.Complete += EndOfAnim;
        //var hitevent = skeletonAnimation.AnimationState.Data.skeletonData.FindEvent("hit");
        //if(hitevent != null)
        //{
        //    Debug.Log($"hitevent {hitevent.Float}__{hitevent.Int}__{hitevent.name}__{hitevent.Name}__{hitevent.String}");
        //}


        //AnimationStateData stateData = new AnimationStateData(skeletonAnimation.state.Data.skeletonData);
        //stateData.DefaultMix = 0.1f;
        //stateData.SetMix("attack/melee/mouth-bite", "action/move-back", 1);
        //Spine.AnimationState state = new Spine.AnimationState(stateData);
        //skeletonAnimation.state = state;

        //skeletonAnimation.AnimationState.SetAnimation(0, "attack/melee/mouth-bite", false);
        //skeletonAnimation.AnimationState.AddAnimation(0, "action/move-back", false, 0);

        //skeletonAnimation.AnimationState.Event += HandleEvent;
        //skeletonAnimation.AnimationState.Start += OnStartEvent;
    }

    private void OnStartEvent(TrackEntry trackEntry)
    {
        Debug.Log($"OnStartEventOnStartEvent__trackEntry_{trackEntry.animation.Name}");
    }

    private void EndOfAnim(TrackEntry trackEntry)
    {
        //Debug.Log($"An animation ended! {trackEntry.animation.Name}");

        //if(trackEntry.animation.Name.Contains("hit"))
        //{
        //    moveBack = true;
        //}
    }

    private void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        Debug.Log($"HandleEvent! e__________{e.Data.Name}");
        Debug.Log($"HandleEvent! trackEntry_{trackEntry.animation.Name}");

        //if (e.Data.Name.Contains("hit"))
        //{
        //    move = false;
        //    moveBack = true;
        //}

        //var hitevent = skeletonAnimation.AnimationState.Data.skeletonData.FindEvent("hit");
        //if (hitevent != null)
        //{
        //    Debug.Log($"hitevent {hitevent.Float}__{hitevent.Int}__{hitevent.name}__{hitevent.Name}__{hitevent.String}");
        //}

    }
    /*
    float delay = 3f;
    float count = 0;
    bool move = false;
    float moveTime;
    float countMoveTime;
    Vector2 backPos;
    float backTime = 1;
    void Update()
    {
        count += Time.deltaTime;
        if (CanPerformAttack() && Input.GetKey("space") && count > delay)
        {
            //skeletonAnimation.AnimationState.SetAnimation(0, "attack/melee/mouth-bite", false);
            skeletonAnimation.AnimationState.SetAnimation(0, "action/move-forward", false);

            Debug.Log($"GetAnimTime____{GetAnimTime()}___{skeletonAnimation.state.GetCurrent(0).Animation.name}");
            moveTime = GetAnimTime();
            move = true;
        }

        if(move)
        {
            //countMoveTime += Time.deltaTime/ moveTime;
            transform.position = Vector2.Lerp(transform.position, EnemyTransform.position, countMoveTime);
            if (countMoveTime > moveTime)
            {
                countMoveTime = 0;
                move = false;
                //moveBack = true;
            }
        }

        if (moveBack)
        {
            //Debug.Log($"backPos {backPos.x}");
            countMoveBackTime += Time.deltaTime/backTime;
            transform.position = Vector2.Lerp(transform.position, backPos, countMoveBackTime);
            if (countMoveBackTime > 1)
            {
                moveBack = false;
                countMoveBackTime = 0;
            }
        }
    }

    bool moveBack = false;
    private float countMoveBackTime = 0;

    private float GetAnimTime()
    {
        return skeletonAnimation.state.GetCurrent(0).Animation.duration;
    }

    private bool CanPerformAttack()
    {
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogError("collision " + collision.gameObject.name);
    }
    */
}

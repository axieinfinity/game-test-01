using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [SpineAnimation("action/idle")]
    public string idleAnimation;

    [SpineAnimation]
    public string attackAnimation;

    [SpineAnimation]
    public string moveForwardAnimation;

    [SpineAnimation]
    public string moveBackwardAnimation;

    public KeyCode attackKey = KeyCode.Space;

    public float moveSpeed = 3;

    SkeletonAnimation skeletonAnimation;


    public Transform EnemyPlayer;

    Vector3 startPosition;
    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(attackKey))
        {
            moveToEnemy();
        }
    }

    void moveToEnemy()
    {
        Vector3 newPos = Vector3.ClampMagnitude(transform.position - EnemyPlayer.position, 1f);
        skeletonAnimation.AnimationState.SetAnimation(0, moveForwardAnimation, true);
        LeanTween.move(gameObject, EnemyPlayer.position + newPos, 1f / moveSpeed).setEase(LeanTweenType.easeOutCubic).setOnComplete(attack);
    }

    void attack()
    {
        //skeletonAnimation.AnimationName = attackAnimation;

        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimation, false);
        skeletonAnimation.AnimationState.End += delegate {
            moveBack();
        };
    }

    void moveBack()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, moveBackwardAnimation, true);
        LeanTween.move(gameObject, startPosition, 1f / moveSpeed).setEase(LeanTweenType.easeOutCubic).setOnComplete(setIdle);
    }

    void setIdle()
    {
        //skeletonAnimation.AnimationName = attackAnimation;

        skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, true);
    }
}

using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    [HideInInspector]
    public int id = -1;

    #region Public variables

    public SkeletonAnimation skeletonAnimation;

    [SpineAnimation("action/idle")]
    public string idleAnimation;

    [SpineAnimation]
    public string attackAnimation;

    [SpineAnimation]
    public string hitByAttackAnimation;

    [SpineAnimation]
    public string moveForwardAnimation;

    [SpineAnimation]
    public string moveBackwardAnimation;

    [SpineAnimation]
    public string victoryAnimation;

    public Image healthBar;

    public float moveSpeed = 3;
    public float timeBite = 1.333f;
    public float smoothLerp = 3f;
    #endregion

    float nextValueHP = 100f;
    float curValueHP = 100f;
    float maxValueHP = 100f;
    PlayerController EnemyPlayer;
    Vector3 startPosition;
    

    void Awake()
    {
        startPosition = transform.position;
        curValueHP = maxValueHP;
        nextValueHP = curValueHP;
    }

    private void Start()
    {
        healthBar.fillAmount = 1f;
    } 

    public void setEnemy(PlayerController enemy)
    {
        EnemyPlayer = enemy;
    }

    public void moveToEnemy()
    {
        if (id == -1) return;
        if (EnemyPlayer == null) return;
        if (skeletonAnimation == null) return;

        Vector3 newPos = Vector3.ClampMagnitude(transform.position - EnemyPlayer.transform.position, 1f);
        skeletonAnimation.AnimationState.SetAnimation(0, moveForwardAnimation, true);
        LeanTween.move(gameObject, EnemyPlayer.transform.position + newPos, 1f / moveSpeed).setEase(LeanTweenType.easeOutCubic).setOnComplete(attack);
    }

    void attack()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, attackAnimation, false);

        EnemyPlayer.setHitByAttack();
        float randomDam = Random.Range(5f, 20f);
        EnemyPlayer.setNextHP(randomDam);

        if(id == BattleControler.instance.myPlayerID)
        {
            SoundManager.instance.playMyPlayerAttack();
        }
        else
        {
            SoundManager.instance.playEnemyPlayerAttack();
        }

        StartCoroutine(waitToMoveBack());
    }

    public void setHitByAttack()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, hitByAttackAnimation, false);
        skeletonAnimation.AnimationState.AddAnimation(1, idleAnimation, true, 0);
    }

    IEnumerator waitToMoveBack()
    {
        yield return new WaitForSeconds(timeBite);
        moveBack();
    }

    void moveBack()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, moveBackwardAnimation, true);
        LeanTween.move(gameObject, startPosition, 1f / moveSpeed).setEase(LeanTweenType.easeOutCubic).setOnComplete(setIdle);
    }

    void setIdle()
    {
        startPosition = transform.position;
        skeletonAnimation.AnimationState.SetAnimation(0, idleAnimation, true);
        if(EnemyPlayer.healthBar.fillAmount == 0)
        {
            setVictory();
            BattleControler.instance.showWinPlayer(id);
        }
        else
        {
            BattleControler.instance.switchPlayerTurn(id);
        }
        
    }

    void setVictory()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, victoryAnimation, true);
    }

    void setNextHP(float numDam)
    {
        nextValueHP = curValueHP - numDam;
        Debug.Log(id + "'s health remain " + curValueHP);
    }

    void setHealthBar()
    {
        float percent = curValueHP / maxValueHP;
        percent = Mathf.Clamp(percent, 0, 1f);

        healthBar.fillAmount = percent;
    }

    private void Update()
    {
        if(curValueHP != nextValueHP)
        {
            if(curValueHP - nextValueHP < 0.02f)
            {
                curValueHP = nextValueHP;                
            }
            else
            {
                curValueHP = Mathf.Lerp(curValueHP, nextValueHP, smoothLerp * Time.deltaTime);
            }
            setHealthBar();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public partial class View : MonoBehaviour
{
    public const float HEXAGON_RADIUS = 1;
    public static Color orange = new Color32(255, 150, 50, 255);
    public const float TIME_HP = 0.25f;

    public SkeletonAnimation defender;
    public SkeletonAnimation attacker;
    public SpriteRenderer[] hpBars;
    public Data data;

    protected SkeletonAnimation currentView;
    protected Action<Data, View> onDieComplete;

    public void Init(Data _data)
    {
        data = _data;

        defender.gameObject.SetActive(data != null && data.type == CellType.Defender);
        attacker.gameObject.SetActive(data != null && data.type == CellType.Attacker);

        if (data.type == CellType.Attacker)
            currentView = attacker;
        else if (data.type == CellType.Defender)
            currentView = defender;

        UpdatePosition();
        UpdateFace();
        UpdateHpBar(TIME_HP);
        
        currentView.AnimationState.End -= OnAnimationEnd;
        currentView.AnimationState.End += OnAnimationEnd;
        currentView.AnimationState.SetAnimation(0, AnimationName.appear, false);
        currentView.AnimationState.SetAnimation(1, AnimationName.idle, true);
    }

    public void UpdatePosition()
    {
        transform.localPosition = CalculatePosition(data.pos, HEXAGON_RADIUS);
    }

    void UpdateFace()
    {
        if ((transform.localPosition.x < 0 && data.type == CellType.Attacker)
            ||
            (transform.localPosition.x > 0 && data.type == CellType.Defender))
        {
            currentView.skeleton.flipX = true;
        }
    }

    void FaceToTarget()
    {
        if (data.target != null)
        {
            var target = CalculatePosition(data.target, HEXAGON_RADIUS);
            var delta = target.x - transform.localPosition.x;
            if (delta > 0)
            {
                currentView.skeleton.flipX = true;
            }
            else
            {
                currentView.skeleton.flipX = false;
            }
        }
    }

    void OnAnimationEnd(TrackEntry trackEntry)
    {
        if (trackEntry.animation.name == AnimationName.die)
        {
            onDieComplete?.Invoke(data, this);
            onDieComplete = null;
        }
    }

    public static Vector2 CalculatePosition(Int3 hex, float size)
    {
        var x = size * (Mathf.Sqrt(3f) * hex.x + Mathf.Sqrt(3f) / 2f * hex.z);
        var y = size * (3f / 2f * hex.z);
        return new Vector2(x, y);
    }

    public void AnimationDie(float time, Action<Data, View> callback)
    {
        transform.DOScale(Vector3.zero, time).OnComplete(() => { callback?.Invoke(this.data, this); });
    }

    public void AnimationMove(float time)
    {
        FaceToTarget();
        transform.DOLocalMove(CalculatePosition(data.target, HEXAGON_RADIUS), time);
    }

    public void AnimationDefende(float time)
    {
        UpdateHpBar(TIME_HP);
        FaceToTarget();

        currentView.AnimationState.SetAnimation(0, AnimationName.defense, false);
        currentView.AnimationState.SetAnimation(1, AnimationName.idle, true);
        
        var ske = currentView.skeleton;
        var current = currentView.skeleton.GetColor();
        var target = Color.red;

        DOTween.To(() => new Vector4(current.r, current.g, current.b, current.a),
            c => { ske.SetColor(new Color(c.x, c.y, c.z, c.w)); },
            new Vector4(target.r, target.g, target.b, target.a), time / 2f).OnComplete(() =>
        {
            DOTween.To(() => new Vector4(target.r, target.g, target.b, target.a),
                c => { ske.SetColor(new Color(c.x, c.y, c.z, c.w)); },
                new Vector4(current.r, current.g, current.b, current.a), time / 2f);
        });
    }

    public void AnimationAttack (float time)
    {
        UpdateHpBar(TIME_HP);
        FaceToTarget();
        
        Vector2 currentPos = transform.localPosition;
        Vector2 target = CalculatePosition(data.target, HEXAGON_RADIUS);
        target = Vector2.Lerp(target, currentPos, 0.25f); //near target, not target
        
        //move to target
        transform.DOLocalMove(target, time / 2f).OnComplete(() => { transform.DOLocalMove(currentPos, time / 2f); });

        currentView.AnimationState.SetAnimation(0, AnimationName.attack, false);
        currentView.AnimationState.SetAnimation(1, AnimationName.idle, true);
    }

    public void AnimationVictory()
    {
        //hide all hp bars
        foreach (SpriteRenderer hpBar in hpBars)
        {
            hpBar.gameObject.SetActive(false);
        }
        
        currentView.AnimationState.AddAnimation(0, AnimationName.victory, true, 0); //wait other animation
    }

    void UpdateHpBar(float time)
    {
        float scale = 1;
        int hp = Math.Max(data.hp, 0);
        if (data.type == CellType.Attacker)
        {
            scale = (float)hp / Constants.HpAttacker;
        }
        else if (data.type == CellType.Defender)
        {
            scale = (float)hp / Constants.HpDefender; 
        }

        scale *= 100;
        Color color = Color.green;
        if (scale < 20)
            color = Color.red;
        else if (scale < 50)
            color = orange;

        foreach (SpriteRenderer hpBar in hpBars)
        {
            hpBar.gameObject.SetActive(true);
            if (hpBar.gameObject.activeInHierarchy)
            {
                DOTween.To(() => new Vector4(hpBar.color.r, hpBar.color.g, hpBar.color.b, hpBar.color.a), c =>
                    {
                        hpBar.color = new Color(c.x, c.y, c.z, c.w);
                    },
                    new Vector4(color.r, color.g, color.b, color.a), 0.25f);
                hpBar.transform.DOScaleX(scale, time);
            }
        }
    }
}

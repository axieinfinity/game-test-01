using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Spine;
using AnimationState = Spine.AnimationState;
using DG.Tweening;

public class Character : MonoBehaviour
{
    public enum CType
    {
        Defender = 0,
        Attacker = 1,

    }

    public CType Type
    {
        get => type;
    }

    public HexaUnit HexaUnit
    {
        get;
        set;
    }

    public static Action<Character> DoAction = delegate {};
    public static Action<Character> DoActionMove = delegate { };
    public static Action<Character, Character> DoAttack = delegate { };
    public static Action<Character> DoDead = delegate { };

    [SerializeField] CType type;
    [SerializeField] HpBar hpBar;

    private Transform _transform;
    private SkeletonAnimation _skeletonAnimation;
    private AnimationState _animationState;
    private int _maxHp;
    private int _currentHp;
    private int _currentTrackIdx;
    private bool _isDead;

    public bool IsDead
    {
        get => _isDead;
    }

    public int CurrentHp
    {
        get => _currentHp;
        set => _currentHp = value;
    }

    public int MaxHp
    {
        get => _maxHp;
    }

    private int _attribute;

    public int Attribute
    {
        get => _attribute;
    }

    private float _currentTimeCount;
    private float _currentMoveTimeCount;

    public void OnSpineAnimationStart(TrackEntry trackEntry)
    {
        // Add your implementation code here to react to start events
    }
    public void OnSpineAnimationInterrupt(TrackEntry trackEntry)
    {
        // Add your implementation code here to react to interrupt events
    }
    public void OnSpineAnimationEnd(TrackEntry trackEntry)
    {
        // Add your implementation code here to react to end events
        if (trackEntry.TrackIndex == _currentTrackIdx)
        {
            //_currentTrackIdx = _currentTrackIdx == 0 ? _currentTrackIdx = 1 : _currentTrackIdx = 0;
            _skeletonAnimation.AnimationState.SetAnimation(_currentTrackIdx+1, "action/idle", true);
        }
    }
    public void OnSpineAnimationDispose(TrackEntry trackEntry)
    {
        // Add your implementation code here to react to dispose events
    }
    public void OnSpineAnimationComplete(TrackEntry trackEntry)
    {
        // Add your implementation code here to react to complete events
        
    }

    public void ResetStats()
    {
        //random attribute number
        _attribute = Random.Range(0, 3);
        _currentTimeCount = 0f;
        _currentMoveTimeCount = Game.Instance.AttackSpeed / 3.0f;

        if (type == CType.Attacker)
        {
            _maxHp = 10;
        }
        else
        {
            _maxHp = 30;
        }

        _currentHp = _maxHp;
    }

    private void Start()
    {
        //cached to use later
        _transform = transform;

        //spine animation
        _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        _animationState = _skeletonAnimation.AnimationState;

        _animationState.Start += OnSpineAnimationStart;
        _animationState.Interrupt += OnSpineAnimationInterrupt;
        _animationState.End += OnSpineAnimationEnd;
        _animationState.Dispose += OnSpineAnimationDispose;
        _animationState.Complete += OnSpineAnimationComplete;

        //random attribute number
        _attribute = Random.Range(0, 3);
        _currentTimeCount = 0f;
        _currentMoveTimeCount = Game.Instance.AttackSpeed / 3.0f;
        _animationState.TimeScale = Game.Instance.AttackSpeed / 2f;

        if (type == CType.Attacker)
        {
            _maxHp = 10;
        } else
        {
            _maxHp = 30;
        }

        _currentHp = _maxHp;
    }

    private void OnDestroy()
    {
        if (_animationState == null)
        {
            return;
        }

        _animationState.Start -= OnSpineAnimationStart;
        _animationState.Interrupt -= OnSpineAnimationInterrupt;
        _animationState.End -= OnSpineAnimationEnd;
        _animationState.Dispose -= OnSpineAnimationDispose;
        _animationState.Complete -= OnSpineAnimationComplete;

        DOTween.Kill(this);
    }

    public void UpdatePosition ()
    {
        //check again for edit mode
        if (_transform == null) _transform = transform;
        _transform.localPosition = HexaUnit.transform.localPosition;

        if (_skeletonAnimation == null) _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        if (_animationState == null) _animationState = _skeletonAnimation.AnimationState;

        //flip characters, make them look like ready to fight with others
        if (_transform.localPosition.x < 0)
        {
            if (type == CType.Attacker)
            {
                _skeletonAnimation.Skeleton.FlipX = true;
            } else
            {
                _skeletonAnimation.Skeleton.FlipX = false;
            }
            
        } else
        {
            if (type == CType.Attacker)
            {
                _skeletonAnimation.Skeleton.FlipX = false;
            }
            else
            {
                _skeletonAnimation.Skeleton.FlipX = true;
            }
        }
    }

    private void Update()
    {
        if (_isDead)
        {
            return;
        }

        if (type == CType.Defender)
        {
            return;
        }

        _currentTimeCount += Time.deltaTime;

        if (_currentTimeCount > Game.Instance.AttackSpeed)
        {
            _currentTimeCount -= Game.Instance.AttackSpeed;

            Action();
        }

        _currentMoveTimeCount += Time.deltaTime;
        if (_currentMoveTimeCount > Game.Instance.AttackSpeed)
        {
            _currentMoveTimeCount -= Game.Instance.AttackSpeed;

            ActionMove();
        }
    }

    private void ActionMove ()
    {
        DoActionMove(this);
    }

    private void Action ()
    {
        //_currentTrackIdx = _currentTrackIdx == 0 ? _currentTrackIdx = 1 : _currentTrackIdx = 0;
        
        DoAction(this);
    }

    public void Move(HexaUnit unit)
    {
        HexaUnit = unit;
        unit.Character = this;

        Sequence seq = DOTween.Sequence();
        seq.Append(_transform.DOLocalMove(unit.transform.localPosition, Game.Instance.AttackSpeed / 10f));
        seq.AppendCallback(() =>
        {
            //flip characters, make them look like ready to fight with others
            if (_transform.localPosition.x < 0)
            {
                if (type == CType.Attacker)
                {
                    _skeletonAnimation.Skeleton.FlipX = true;
                }
                else
                {
                    _skeletonAnimation.Skeleton.FlipX = false;
                }

            }
            else
            {
                if (type == CType.Attacker)
                {
                    _skeletonAnimation.Skeleton.FlipX = false;
                }
                else
                {
                    _skeletonAnimation.Skeleton.FlipX = true;
                }
            }
        });
        seq.SetTarget(this);
    }

    public void Attack(Character character)
    {
        if (type == CType.Attacker) {
            _skeletonAnimation.AnimationState.SetAnimation(_currentTrackIdx, "attack/melee/back-gore", false);
            DoAttack(this, character);
            character.Attack(null);
        } else
        {
            _skeletonAnimation.AnimationState.SetAnimation(_currentTrackIdx, "attack/melee/back-gore", false);
        }
    }

    public void Damaged (int dmg)
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(Game.Instance.AttackSpeed / 10f);
        seq.AppendCallback(() =>
        {
            _currentHp -= dmg;

            if (_currentHp <= 0)
            {
                Dead();
            }

            float ratio = (float)_currentHp / (float)_maxHp;
            hpBar.UpdateRatio(ratio);
        });
        seq.SetTarget(this);

        
    }

    private void Dead ()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        //Sequence seq = DOTween.Sequence();
        //seq.AppendInterval(Game.Instance.AttackSpeed / 10f);
        //seq.AppendCallback(() =>
        //{
            DoDead(this);
        //});
        //seq.SetTarget(this);
        
    }
}

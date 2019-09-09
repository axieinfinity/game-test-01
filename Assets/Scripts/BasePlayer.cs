using System;
using Spine;
using Spine.Unity;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    private const int ATTACK_RENDER_ORDER = 10;
    private const int BASE_RENDER_ORDER = 5;

    protected const int ANIM_IDLE_ID = 0;
    protected const int ANIM_MOVE_FORWARD_ID = 1;
    protected const int ANIM_MOVE_BACK_ID = 2;

    protected const int ANIM_ATTACK_BITE_ID = 5;
    protected const int ANIM_ATTACK_HORN_ID = 6;

    protected const int ANIM_DEFENSE_BITE_ID = 10;
    protected const int ANIM_DEFENSE_DRAMATIC = 11;

    [SerializeField] public float _moveSpeed;
    [SerializeField] public SkeletonAnimation _skeletonAnimation;
    [SerializeField] public Transform _hitPos;
    [SerializeField] public ColliderComponent[] _colliders;
    [SerializeField] public HealthBar _healthBar;

    public Action<BasePlayer> OnTriggerHit;
    public Action<BasePlayer> BackToBeginPosition;
    public Action<BasePlayer> OnDead;

    private ACTION_STATE _currentState;
    public ACTION_STATE CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
            SetColliderState();
            SetOrderState();
        }
    }

    protected Vector2 _beginPosition;
    protected bool _checkHit = false;
    protected ACTION_PHASE _currentMotionPhase;
    protected BasePlayer _target;
    protected int _attackId;
    protected MeshRenderer _mesh; 

    protected TrackEntry _currentTrack;

    // attribute
    private int _maxHp;
    public int CurrentHp { get; private set; }
    public int Damage { get; private set; }

    private void Start()
    {
        _beginPosition = new Vector2(transform.position.x, transform.position.y);

        if (_colliders != null)
        {
            foreach (var c in _colliders)
            {
                c.OnTrigger += OnTrigger;
            }
        }
        _mesh = _skeletonAnimation.gameObject.GetComponent<MeshRenderer>();
    }

    public virtual void Init(int hp, int damage, ACTION_STATE initState, BasePlayer target)
    {
        AnimationStateData stateData = new AnimationStateData(_skeletonAnimation.state.Data.skeletonData);
        stateData.DefaultMix = 0.1f;
        stateData.SetMix(
            AnimationHelper.Instance.GetAnimDetailName(ANIM_ATTACK_BITE_ID),
            AnimationHelper.Instance.GetAnimDetailName(ANIM_MOVE_BACK_ID),
            0.8f);
        stateData.SetMix(
           AnimationHelper.Instance.GetAnimDetailName(ANIM_ATTACK_HORN_ID),
           AnimationHelper.Instance.GetAnimDetailName(ANIM_MOVE_BACK_ID),
           0.8f);

        Spine.AnimationState state = new Spine.AnimationState(stateData);

        _skeletonAnimation.state = state;
        DoAnimIdle();

        _skeletonAnimation.AnimationState.Event += OnEvent;
        _skeletonAnimation.AnimationState.Start += OnStartTrack;

        _maxHp = hp;
        CurrentHp = _maxHp;
        Damage = damage;
        CurrentState = initState;
        _target = target;

        _checkHit = true;
        _healthBar.Init(_maxHp);

        SetColliderState();
    }

    public void Reset(ACTION_STATE state)
    {
        CurrentHp = _maxHp;
        _checkHit = true;
        CurrentState = state;
        _healthBar.Reset();
    }

    private void OnStartTrack(TrackEntry trackEntry)
    {
        //Debug.LogError($" ++++++++++++++ " + trackEntry.Animation.name + "__ Object::" + gameObject.name);
        if (trackEntry.Animation.name == AnimationHelper.Instance.GetIdleAnimDetail().Anim)
        {
            _checkHit = true;
        }

        if (trackEntry.animation.Name == AnimationHelper.Instance.GetAnimDetailName(ANIM_MOVE_BACK_ID))
        {
            _currentMotionPhase = ACTION_PHASE.MOVE_BACK;
            AddAnim(ANIM_IDLE_ID, true);
        }
    }

    private void OnTrigger(GameObject sender, Collider2D obj)
    {
        if (_checkHit)
        {
            _checkHit = false;
            OnTriggerHit?.Invoke(this);
        }
    }

    protected virtual void OnEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name.Contains("hit"))
        {
            SoundManager.Instance.Play("bite");
        }
    }

    public void DoAttack(int damage)
    {
    }

    public void DoBeAttacked(int damage)
    {
        CurrentHp -= damage;

        if (CurrentHp <= 0)
            OnDead?.Invoke(this);

        DoAnim(GetHitAnimId(), true);
        UpdateHealthBar(damage);
    }

    private int GetHitAnimId()
    {
        return CurrentHp > 0 ? ANIM_DEFENSE_BITE_ID : ANIM_DEFENSE_DRAMATIC;
    }

    protected void AddAnim(int animId, bool idleAfter = false)
    {
        var anim = AnimationHelper.Instance.GetAnimDetailName(animId);
        if (!string.IsNullOrEmpty(anim))
            _currentTrack = _skeletonAnimation.AnimationState.AddAnimation(0, anim, true, 0);
    }

    protected void DoAnim(int animId, bool idleAfter = false)
    {
        var anim = AnimationHelper.Instance.GetAnimDetailName(animId);
        if (!string.IsNullOrEmpty(anim))
            _currentTrack = _skeletonAnimation.AnimationState.SetAnimation(0, anim, false);
        if (idleAfter)
            AddAnim(ANIM_IDLE_ID, true);
        //_skeletonAnimation.AnimationState.AddAnimation(0, idleAnim, true, 0);
    }

    protected void DoAnimIdle(bool loop = true)
    {
        var anim = AnimationHelper.Instance.GetAnimDetailName(ANIM_IDLE_ID);
        if (!string.IsNullOrEmpty(anim))
            _currentTrack = _skeletonAnimation.AnimationState.SetAnimation(0, anim, loop);
    }

    private void UpdateHealthBar(int damage)
    {
        _healthBar.GetHit(damage);
    }

    protected void SetColliderState()
    {
        if (CurrentState == ACTION_STATE.ATTACK)
            DisableAllCollider();
        else if (CurrentState == ACTION_STATE.DEFENCE)
            EnableCollider(ANIM_IDLE_ID);
    }

    protected void SetOrderState()
    {
        if (CurrentState == ACTION_STATE.ATTACK)
        {
            _mesh.sortingOrder = ATTACK_RENDER_ORDER;
            _healthBar.transform.SetAsLastSibling();
        }
        else if (CurrentState == ACTION_STATE.DEFENCE)
            _mesh.sortingOrder = BASE_RENDER_ORDER;
    }

    protected void DisableAllCollider()
    {
        foreach (var item in _colliders)
        {
            item.enabled = false;
        }
    }

    protected void EnableCollider(int animId)
    {
        var animDetail = AnimationHelper.Instance.GetAnimDetail(animId);
        if(animDetail != null)
        {
            foreach (var item in _colliders)
            {
                item.enabled = false;
                foreach (var colliderName in animDetail.ColliderNames)
                {
                    if (item.gameObject.name.Contains(colliderName))
                    {
                        item.enabled = true;
                        break;
                    }
                }
            }
        }
    }

    //protected virtual void Attack() { }
    //protected virtual void BackToIdle() { }

    private void Update()
    {
        if (_currentMotionPhase == ACTION_PHASE.MOVE_FORWARD)
        {
            float step = _moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _target._hitPos.transform.position, step);

            if (Vector3.Distance(transform.position, _target._hitPos.transform.position) < 0.001f)
            {
                Attack();
            }
        }

        if (_currentMotionPhase == ACTION_PHASE.MOVE_BACK)
        {
            float step = _moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _beginPosition, step);

            if (Vector3.Distance(transform.position, _beginPosition) < 0.001f)
            {
                BackToIdle();
            }
        }
    }

    public void DoAttack()
    {
        if (CurrentState == ACTION_STATE.ATTACK && _currentMotionPhase == ACTION_PHASE.NONE)
        {
            MoveToTarget();
        }
    }

    private int GetAttackAnimId()
    {
        var randomId = UnityEngine.Random.Range(0, 10);
        if (randomId <= 5)
            return ANIM_ATTACK_BITE_ID;
        else
            return ANIM_ATTACK_HORN_ID;
    }

    protected virtual void Attack()
    {
        _attackId = GetAttackAnimId();
        EnableCollider(_attackId);
        PlayAttackAnim(_attackId);
    }

    private void PlayAttackAnim(int attackId)
    {
        _currentMotionPhase = ACTION_PHASE.ATTACK;

        DoAnim(attackId);
        AddAnim(ANIM_MOVE_BACK_ID);
    }

    private void MoveToTarget()
    {
        DoAnim(ANIM_MOVE_FORWARD_ID);
        _currentMotionPhase = ACTION_PHASE.MOVE_FORWARD;
    }

    protected virtual void BackToIdle()
    {
        DoAnimIdle();

        _currentMotionPhase = ACTION_PHASE.NONE;
        _checkHit = true;

        BackToBeginPosition?.Invoke(this);
    }

    public enum ACTION_PHASE
    {
        NONE,
        MOVE_FORWARD,
        MOVE_BACK,
        ATTACK,
    }

    public enum ACTION_STATE
    {
        DEFENCE,
        ATTACK,
    }
}

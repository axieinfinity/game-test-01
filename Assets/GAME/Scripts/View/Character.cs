using System;
using Spine;
using Spine.Unity;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public static partial class Constant
{
    public static class CharacterView
    {
        public const string MoveForward = "action/move-forward";
        public const string MoveBack = "action/move-back";
        public const string AttackByRanged = "attack/ranged/cast-fly";
        public const string DefenseByRanged = "defense/hit-by-ranged-attack";
        public const string DefenseByNormal = "defense/hit-by-normal-attack";
        public const string AttackMouthBite = "attack/melee/mouth-bite";
        public const string Idle = "action/idle";
        public const float DelayFromStartedToBitten = 0.5f;
    }
}

public class Character : MonoBehaviour,
    IView<CharacterModel>,
    IModel //for data from unity Transform etc..
{
    [SerializeField] private SkeletonAnimation _skeletonAnimation;

    private Transform _transform;

    public Transform Transform
    {
        get
        {
            if (_transform == null)
            {
                _transform = GetComponent<Transform>();
            }

            return _transform;
        }
        set => _transform = value;
    }


    public void UpdateView(SkeletonDataAsset skeletonDataAsset)
    {
        _skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
        skeletonDataAsset.Clear();
        skeletonDataAsset.GetSkeletonData(true);
        _skeletonAnimation.Initialize(true);
    }

    public async UniTask Attack(AttackParameters parameters)
    {
        parameters.Character = this;
        _skeletonAnimation.AnimationState.Start += OnAttackingAnimStarted;
        _skeletonAnimation.AnimationState.Complete += OnAttackingAnimCompleted;

        async void OnAttackingAnimStarted(TrackEntry entry)
        {
            if (!entry.animation.name.Equals(Constant.CharacterView.AttackMouthBite)) return;
            await UniTask.Delay(TimeSpan.FromSeconds(Constant.CharacterView.DelayFromStartedToBitten));
            parameters.OnTargetBitten?.Invoke(parameters);
            _skeletonAnimation.AnimationState.Start -= OnAttackingAnimStarted;
        }

        void OnAttackingAnimCompleted(TrackEntry entry)
        {
            if (!entry.animation.name.Equals(Constant.CharacterView.AttackMouthBite)) return;
            parameters.OnActionFinished?.Invoke(parameters);
            _skeletonAnimation.AnimationState.Complete -= OnAttackingAnimCompleted;
        }

        PlayMoveForwardAnim();
        ObservableTween.Tween(transform.position,
                parameters.TargetPos,
                parameters.MoveDuration,
                ObservableTween.EaseType.Linear)
            .Subscribe(pos => transform.position = pos);
        await UniTask.Delay(TimeSpan.FromSeconds(parameters.MoveDuration));

        PlayAttackMeleeAnim();
    }


    public void HitByAttack(BittenParameters parameters)
    {
        parameters.Character = this;
        _skeletonAnimation.AnimationState.Complete += OnAnimationStateOnEnd;

        void OnAnimationStateOnEnd(TrackEntry entry)
        {
            if (!entry.animation.name.Equals(Constant.CharacterView.DefenseByNormal))
            {
                return;
            }

            parameters.OnBittenFinished?.Invoke(parameters);
            PlayIdleAnim();
            _skeletonAnimation.AnimationState.Complete -= OnAnimationStateOnEnd;
        }

        Debug.Log("[CharacterView] --> will play hit by attack animation.");
        PlayBittenAnim();
    }


    public async UniTask ComeBack(MovingParameters parameters)
    {
        parameters.Character = this;
        PlayMoveBackAnim();
        ObservableTween.Tween(Transform.position,
                parameters.TargetPos,
                parameters.MoveDuration,
                ObservableTween.EaseType.Linear)
            .Subscribe(pos => Transform.position = pos);
        await UniTask.Delay(TimeSpan.FromSeconds(parameters.MoveDuration));
        parameters.OnActionFinished?.Invoke(parameters);
        PlayIdleAnim();
    }

    private void PlayMoveBackAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0,
            Constant.CharacterView.MoveBack,
            true);
    }

    private void PlayAttackMeleeAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0,
            Constant.CharacterView.AttackMouthBite,
            false);
    }

    private void PlayBittenAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0,
            Constant.CharacterView.DefenseByNormal,
            false);
    }

    public void PlayIdleAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0,
            Constant.CharacterView.Idle,
            true);
    }

    private void PlayMoveForwardAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0,
            Constant.CharacterView.MoveForward,
            true);
    }

    public void SetSortingOrder(int order)
    {
        GetComponent<Renderer>()
            .sortingOrder = order;
    }

    public int GetSortingOrder()
    {
        return GetComponent<Renderer>()
            .sortingOrder;
    }

    public void UpdateView(CharacterModel model)
    {
    }

    public class Pool : MonoMemoryPool<Character>
    {
    }
}


public class BittenParameters : BaseParameters<BittenParameters>
{
    public UnityAction<BittenParameters> OnBittenFinished { get; set; }
}

public class AttackParameters : MovingParameters<AttackParameters>
{
    public UnityAction<AttackParameters> OnTargetBitten;
    public Character Target;
}

public class MovingParameters : MovingParameters<MovingParameters>
{
}


public abstract class MovingParameters<T> : BaseParameters<T> where T : MovingParameters<T>
{
    public float MoveDuration { get; set; }
    public Vector3 TargetPos { get; set; }
    public Vector3 StartPos { get; set; }
}

public abstract class BaseParameters<T> where T : BaseParameters<T>
{
    public Character Character;
    public UnityAction<T> OnActionFinished;
    public UnityAction<T> OnActionStarted;
}
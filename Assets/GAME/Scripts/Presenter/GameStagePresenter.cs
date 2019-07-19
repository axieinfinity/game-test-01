using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public static partial class Constant
{
    public static class GameStagePresenter
    {
        public const float HPBarHeightOnScreen = 100.0f;
        public const float MoveDuration = 1.0f;
        public const float DistanceNearTarget = -1.0f;
    }
}


public class GameStagePresenter : IGameStagePresenter
{
    [Inject] private Character.Pool _characterViewPool;
    [Inject] private IFactory<Stage> _stageFac;
    [Inject] private HPBarView.Pool _hpBarViewPool;
    [Inject(Id = Constant.ZenjectId.MainCamera)] private Camera _mainCamera;
    [Inject(Id = Constant.ZenjectId.WorldUIRoot)] private Transform _worldUIRoot;

    private ISubject<bool> _gameOverSubject = new Subject<bool>();

    private List<CharacterModel> _characterModelList;
    private StageModel _stageModel;
    private SpineData[] _spineDataArray;
    private readonly Dictionary<string, Character> _characterViews = new Dictionary<string, Character>();
    private Dictionary<string, HPBarView> _hpBarViews = new Dictionary<string, HPBarView>();

    private string playerDataId;

    private Stage _stage;
    private int _maxOrder;
    private CancellationTokenSource _updatingHPBarToken;
    private CancellationTokenSource _processAttackingToken;
    private GameConfig _gameConfig;

    private Vector3 _playerSpawnPos;
    private Vector3 _enemySpawnPos;

    private Vector3 _offsetY = new Vector3(0,
        1.5f,
        0);

    private bool _isAttacking = false;

    private int IncMax()
    {
        return ++_maxOrder;
    }

    public IObservable<bool> OnGameOverAsObservable()
    {
        return _gameOverSubject.AsObservable();
    }


    public IGameStagePresenter SetSpineData(SpineData[] spineData)
    {
        _spineDataArray = spineData;
        return this;
    }

    public IGameStagePresenter SetDataIds(string[] idArray,
        string playerDataId)
    {
        this.playerDataId = playerDataId;
        return this;
    }

    public IGameStagePresenter Init()
    {
        //init models
        _characterModelList = new List<CharacterModel>();
        Enumerable.Range(0,
                _gameConfig.PlayerCharacterNumber)
            .ToObservable()
            .Subscribe(i =>
            {
                var character = new CharacterModel
                {
                    Id = $"p-char{i}".ToString(),
                    Damage = 100,
                    IsPlayer = true,
                    SpineDataId = playerDataId
                };
                _characterModelList.Add(character);
            });

        Enumerable.Range(0,
                _gameConfig.EnemyCharacterNumber)
            .ToObservable()
            .Subscribe(i =>
            {
                var ids = _spineDataArray.Where(data => data.Id != playerDataId)
                    .Select(data => data.Id)
                    .ToList();
                var character = new CharacterModel
                {
                    Id = $"e-char{i}".ToString(),
                    Damage = 100,
                    IsPlayer = false,
                    SpineDataId = ids[Random.Range(0,
                        ids.Count)]
                };
                _characterModelList.Add(character);
            });

        _stageModel = new StageModel
        {
            GroundRotationX = 60,
            GroundPositionY = -2.0f,
            SpriteRendererHeight = 20.0f,
            SpriteRendererWidth = 20.0f
        };
        return this;
    }

    //render the stage
    public void UpdateView()
    {
        if (_stage == null)
        {
            _stage = _stageFac.Create();
            _stage.UpdateView(_stageModel);
            _playerSpawnPos = _stage.PlayerPos;
            _enemySpawnPos = _stage.EnemyPos;
        }

        if (_characterViews == null || _characterViews.Count == 0)
        {
            InitCharacterViews();
        }
    }

    private void InitCharacterViews()
    {
        CreateViews();

        //play init anim
        _characterViews.ToObservable()
            .Subscribe(character => character.Value.PlayIdleAnim());

        //loop: update hp bar position
        _updatingHPBarToken = new CancellationTokenSource();
        UpdateHPBarPosAsync(_updatingHPBarToken.Token)
            .SuppressCancellationThrow();

        //subscribe to reactive properties to update views
        _characterModelList.Join(_hpBarViews,
                model => model.Id,
                hpBar => hpBar.Key,
                (model,
                    hpBar) => (model, hpBar.Value))
            .ToObservable()
            .Subscribe(pair =>
            {
                var (model, value) = pair;
                model.HP.Subscribe(hp => value.UpdateView(hp));
            });
        _characterModelList.ForEach(model =>
        {
            model.IsDead.Where(isDead => isDead)
                .Subscribe(b => OnDead(model));
        });
    }

    private void OnDead(CharacterModel model)
    {
        Debug.Log($"[GameStagePresenter] --> OnDead {model.Id} heath {model.HP.Value}");
        _characterViewPool.Despawn(_characterViews[model.Id]);
        _hpBarViewPool.Despawn(_hpBarViews[model.Id]);

        _characterViews.Remove(model.Id);
        _hpBarViews.Remove(model.Id);
        _characterModelList.RemoveAll(characterModel => characterModel.Id.Equals(model.Id));

        //check win/lose 
        if (_characterModelList.All(mode => mode.IsPlayer))
        {
            _gameOverSubject.OnNext(true);
        }
        else if (_characterModelList.All(mode => !mode.IsPlayer))
        {
            _gameOverSubject.OnNext(false);
        }

        _processAttackingToken.Cancel();
    }

    private async UniTask UpdateHPBarPosAsync(CancellationToken cancellationToken)
    {
        var characterBarPair = _hpBarViews.Join(_characterViews,
            hpBar => hpBar.Key,
            character => character.Key,
            (hpBar,
                character) => new {HpBar = hpBar.Value, Character = character.Value});
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            characterBarPair
                .ToObservable()
                .Subscribe(pair => pair.HpBar.UpdatePos(_worldUIRoot.GetComponent<RectTransform>()
                    .GetWorldPosInsideRectTransformWithSameScreenPosition(_mainCamera,
                        _mainCamera,
                        pair.Character.Transform.position,
                        0.0f,
                        Constant.GameStagePresenter.HPBarHeightOnScreen)));
            await UniTask.Yield();
        }
    }

    public void InvokePlayerAttack()
    {
        if (_isAttacking)
        {
            Debug.LogWarning("[GameStagePresenter] -- is attacking");
            return;
        }

        var playerCharacterIds = _characterModelList.Where(model => model.IsPlayer)
            .Select(model => model.Id)
            .ToList();
        var enemyCharacterIds = _characterModelList.Where(model => !model.IsPlayer)
            .Select(model => model.Id)
            .ToList();
        var attackerId = playerCharacterIds[Random.Range(0,
            playerCharacterIds.Count)];
        var targetId = enemyCharacterIds[Random.Range(0,
            enemyCharacterIds.Count)];
        _processAttackingToken = new CancellationTokenSource();
        ProcessAttack(attackerId,
                targetId,
                _processAttackingToken.Token)
            .Forget(_ => { _isAttacking = false; });
    }

    private Vector3 GetStartMovingPos(Character character)
    {
        return character.Transform.position;
    }

    private Vector3 GetStopMovingPos(Character character)
    {
        return character.Transform.position +
               character.Transform.TransformDirection(Vector3.right) * Constant.GameStagePresenter.DistanceNearTarget;
    }


    private async UniTask ProcessAttack(string attackerId,
        string targetId,
        CancellationToken processAttackingToken)
    {
        _isAttacking = true;
        var attackerView = _characterViews[attackerId];
        var targetView = _characterViews[targetId];
        var isLock = true;

        //do when target bitten
        void OnTargetBitten(AttackParameters parameters)
        {
            //play hit anim on target
            parameters.Target.HitByAttack(new BittenParameters
            {
                OnBittenFinished = bittenParameters =>
                {
                    Debug.Log("[AttackingLogic] --> Finished bitten anim");
                    //apply dam
                    var charId = _characterViews.FirstOrDefault(pair => pair.Value == parameters.Character)
                        .Key;
                    var tarId = _characterViews.FirstOrDefault(pair => pair.Value == parameters.Target)
                        .Key;
                    var charModel = _characterModelList.FirstOrDefault(model => model.Id.Equals(charId));
                    var targModel = _characterModelList.FirstOrDefault(model => model.Id.Equals(tarId));
                    if (charModel == null || targModel == null)
                    {
                        return;
                    }

                    targModel.HP.Value -= Random.Range(5,
                        charModel.Damage + 1);
                }
            });
        }

        //enemy fight back
        void OnTargetFightBack(MovingParameters parameters)
        {
            targetView.Attack(new AttackParameters
            {
                MoveDuration = Constant.GameStagePresenter.MoveDuration,
                StartPos = GetStartMovingPos(targetView),
                TargetPos = GetStopMovingPos(attackerView),
                OnTargetBitten = OnTargetBitten,
                Target = attackerView,
                OnActionFinished = p =>
                {
                    p.Character.ComeBack(new MovingParameters
                    {
                        MoveDuration = Constant.GameStagePresenter.MoveDuration,
                        StartPos = p.TargetPos,
                        TargetPos = p.StartPos,
                        OnActionFinished = _ => isLock = false
                    });
                }
            });
        }


        attackerView.Attack(new AttackParameters
        {
            MoveDuration = Constant.GameStagePresenter.MoveDuration,
            StartPos = GetStartMovingPos(attackerView),
            TargetPos = GetStopMovingPos(targetView),
            OnTargetBitten = OnTargetBitten,
            Target = targetView,
            OnActionFinished = p =>
            {
                p.Character.ComeBack(new MovingParameters
                {
                    MoveDuration = Constant.GameStagePresenter.MoveDuration,
                    StartPos = p.TargetPos,
                    TargetPos = p.StartPos,
                    OnActionFinished = OnTargetFightBack
                });
            },
        });

        while (isLock)
        {
            processAttackingToken.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }

        _isAttacking = false;
    }


    public IGameStagePresenter SetConfig(GameConfig gameConfig)
    {
        _gameConfig = gameConfig;
        return this;
    }


    private void CreateViews()
    {
        _characterModelList.ToObservable()
            .Subscribe(model => Debug.Log($"model id: {model.Id}"));
        _spineDataArray.ToObservable()
            .Subscribe(data => Debug.Log($"data id: {data.Id}"));
        var characters = from model in _characterModelList
            join spineData in _spineDataArray on model.SpineDataId equals spineData.Id
            select (spineData, model);
        if (!characters.Any())
        {
            Debug.LogWarning("[GameStagePresenter] CreateCharacterViews : No matched Id!");
            return;
        }

        var incMax = IncMax();
        characters.ToObservable()
            .Subscribe(characterData =>
            {
                //create and add character view
                var characterView = CreateCharacterView(characterData,
                    incMax);
                _characterViews.Add(characterData.model.Id,
                    characterView);
                //create and add hp bar view
                var hpBarView = _hpBarViewPool.Spawn();
                _hpBarViews.Add(characterData.model.Id,
                    hpBarView);
            });
    }


    private Character CreateCharacterView((SpineData spineData, CharacterModel model) characterData,
        int sortingOrder)
    {
        var characterView = _characterViewPool.Spawn();
        characterView.gameObject.name = characterData.model.Id;
        //update view by data asset and model
        characterView.UpdateView(characterData.spineData.SkeletonDataAsset);

        //and set the view at right position
        characterView.transform.position = characterData.model.IsPlayer
            ? _playerSpawnPos
            : _enemySpawnPos;

        if (characterData.model.IsPlayer)
        {
            _playerSpawnPos += _offsetY;
        }
        else
        {
            _enemySpawnPos += _offsetY;
        }


        characterView.SetSortingOrder(characterData.model.IsPlayer
            ? sortingOrder + 1
            : sortingOrder);

        //set scale to right value
        var localScale = characterView.transform.localScale;
        localScale.x = characterData.model.IsPlayer
            ? Constant.CharacterModel.ScaleXPlayer
            : 1;
        characterView.Transform.rotation = Quaternion.Euler(0,
            characterData.model.IsPlayer
                ? Constant.CharacterModel.RotationYPlayer
                : 0,
            0);
        characterView.transform.localScale = localScale;
        return characterView;
    }

    public void Clear()
    {
        _characterViews.ToObservable()
            .Subscribe(pair => { _characterViewPool.Despawn(pair.Value); });
        _characterViews.Clear();
        _hpBarViews.ToObservable()
            .Subscribe(pair => _hpBarViewPool.Despawn(pair.Value));
        _hpBarViews.Clear();
        _characterModelList.Clear();
        _characterViewPool.Clear();
        _hpBarViewPool.Clear();
        GameObject.Destroy(_stage.gameObject);
    }
}
using UniRx;
using UnityEngine;
using Zenject;

public class ProgramLogic : MonoBehaviour
{
    [Inject] private ResourceLoader _resourceLoader;
    [Inject] private ResourceCreator _resourceCreator;

    [Inject] private LoadingMenu.Factory _loadingMenuFac;
    [Inject] private PlayMenu.Factory _playMenuFac;
    [Inject] private GameOverMenu.Factory _gameOverMenuFac;
    [Inject] private IFactory<IGameStagePresenter> _gameStagePresenterFac;
    [Inject] private Character.Pool _characterPool;
    [Inject] private HPBarView.Pool _hpBarViewPool;

    private IGameStagePresenter _gameStagePresenter;
    private SpineData[] _spineData;
    private string[] _dataIds;
    private string _playerDataId;

    // Start is called before the first frame update
    async void Start()
    {
        _loadingMenuFac.Create()
            .Open();
        Debug.Log("[ProgramLogic] downloading data...");
        //download and cache data
        _dataIds = new[] {"3999", "4000"};
        _playerDataId = "3999";
        _spineData = await _resourceLoader.LoadCharacterData(_dataIds);

        Debug.Log("[ProgramLogic] creating skeleton data asset ...");
        //create spine asset and cache
        _spineData.ToObservable()
            .Subscribe(data => data.SkeletonDataAsset = _resourceCreator.CreateSkeletonDataAsset(data));
        Debug.Log("[ProgramLogic] --> Done.");

        _gameStagePresenter = _gameStagePresenterFac.Create();
        InitGame();

        var playMenu = _playMenuFac.Create();
        playMenu.Open();
        playMenu.OnClickBtnPlayAsObservable()
            .Subscribe(_ =>
            {
                playMenu.Close();
                Play();
            });

        RegisterGameOver();
    }

    private void RegisterGameOver()
    {
        //logic when game over
        _gameStagePresenter.OnGameOverAsObservable()
            .Subscribe(isWin =>
            {
                var _gameOverMenu = _gameOverMenuFac.Create();
                _gameOverMenu.Open(isWin);
                _gameOverMenu.OnClickBtnPlayAsObservable()
                    .Subscribe(_ =>
                    {
                        _gameStagePresenter.Clear();
                        _gameStagePresenter = _gameStagePresenterFac.Create();
                        InitGame();
                        RegisterGameOver();
                        Play();
                        _gameOverMenu.Close();
                    });
            });
    }

    private void InitGame()
    {
        _characterPool.Resize(50);
        _hpBarViewPool.Resize(50);
        //create character views and present the game stage
        _gameStagePresenter
            .SetSpineData(_spineData)
            .SetDataIds(_dataIds,
                _playerDataId)
            .SetConfig(new GameConfig
            {
                PlayerCharacterNumber = 5,
                EnemyCharacterNumber = 5
            })
            .Init()
            .UpdateView();
    }

    private void Play()
    {
        //listen input from player
        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyUp(KeyCode.Space))
            .Subscribe(_ =>
            {
                //attack enemies
                _gameStagePresenter.InvokePlayerAttack();
            });
    }
}
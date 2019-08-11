using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float DELAY_ENEMY_ATTACK = 1.5f;

    private int _baseHp = 100;

    private int _baseOfsset = 16;
    private int _baseSelfDamage = 20;
    private int _baseeEnemyDamage = 20;

    [SerializeField] private Self _self;
    [SerializeField] private Enemy _enemy;
    [SerializeField] private ResultPopup _result;

    private BasePlayer _attacker;
    private BasePlayer _defender;

    private bool _isGameEnded = false;

    private void Start()
    {
        _self.OnTriggerHit += OnPlayerHit;
        _self.BackToBeginPosition += OnBackToBeginPosition;
        _self.OnDead += OnDead;

        _enemy.OnTriggerHit += OnPlayerHit;
        _enemy.BackToBeginPosition += OnBackToBeginPosition;
        _enemy.OnDead += OnDead;

        _self.Init(_baseHp, _baseSelfDamage, BasePlayer.ACTION_STATE.ATTACK, _enemy);
        _enemy.Init(_baseHp, _baseeEnemyDamage, BasePlayer.ACTION_STATE.DEFENCE, _self);

        _attacker = _self;
        _defender = _enemy;

        _isGameEnded = false;
        _result.Hide();
    }

    private void OnDead(BasePlayer obj)
    {
        // handle some1 dead
        _slowTimeEffect = true;
        Debug.Log("OnDeadOnDead");
    }

    private void OnBackToBeginPosition(BasePlayer obj)
    {
        _isGameEnded = CheckIfEndGame();
        if (!_isGameEnded)
        {
            SwitchPlayerPhase();
        }
    }

    private void SwitchPlayerPhase()
    {
        if(_self.CurrentState == BasePlayer.ACTION_STATE.ATTACK)
        {
            _self.CurrentState = BasePlayer.ACTION_STATE.DEFENCE;
            _enemy.CurrentState = BasePlayer.ACTION_STATE.ATTACK;
            _defender = _self;
            _attacker = _enemy;
        }
        else
        {
            _self.CurrentState = BasePlayer.ACTION_STATE.ATTACK;
            _enemy.CurrentState = BasePlayer.ACTION_STATE.DEFENCE;
            _attacker = _self;
             _defender = _enemy;
        }

        if (_attacker == _enemy)
            StartCoroutine(IEPerformEnemyAttack());
    }

    private bool CheckIfEndGame()
    {
        if(_defender.CurrentHp <= 0) // some 1 die
        {
            var win = _defender != _self;

            EndGame(win);

            return true;
        }
        return false;
    }

    private void EndGame(bool isWin)
    {
        _result.Show(isWin);

        SoundManager.Instance.Play(isWin? "win":"lose");
    }

    private void OnPlayerHit(BasePlayer playerHit)
    {
        if(playerHit == _attacker)
        {
            HandleDoAttack(_attacker);
        }

        if(playerHit == _defender)
        {
            HandleBeAttacked(_defender);
        }
    }

    private void HandleDoAttack(BasePlayer player)
    {

    }

    private void HandleBeAttacked(BasePlayer player)
    {
        player.DoBeAttacked(GetHitDamage(_attacker));
    }

    private int GetHitDamage(BasePlayer player)
    {
        var damage = player.Damage - UnityEngine.Random.Range(0, _baseOfsset);
        return damage;
    }

    private void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            DoSelfAttack();
        }

        CheckSlow(Time.deltaTime);
    }

    private void DoSelfAttack()
    {
        if (_isGameEnded) return;

        if (_attacker == _self)
            _self.DoAttack();
    }

    private void DoEnemyAttack()
    {
        if (_isGameEnded) return;

        if (_attacker == _enemy)
            _enemy.DoAttack();
    }

    IEnumerator IEPerformEnemyAttack()
    {
        yield return new WaitForSeconds(DELAY_ENEMY_ATTACK);
        DoEnemyAttack();
    }

    public void Reset()
    {
        _result.Hide();

        _self.Reset(BasePlayer.ACTION_STATE.ATTACK);
        _enemy.Reset(BasePlayer.ACTION_STATE.DEFENCE);

        _attacker = _self;
        _defender = _enemy;

        _isGameEnded = false;
    }

    float _minSlowTimeScale = 0.2f;
    float _slowTime = 0.7f;
    float _countSlowTime = 0;
    bool _slowTimeEffect = false;
    private void CheckSlow(float deltaTime)
    {
        if(_slowTimeEffect)
        {
            _countSlowTime += deltaTime;
            var lerpValue = Mathf.Lerp(_minSlowTimeScale, 1, _countSlowTime / _slowTime);
            Time.timeScale = lerpValue * lerpValue;
            if (_countSlowTime >= _slowTime)
            {
                _countSlowTime = 0;
                Time.timeScale = 1;
                _slowTimeEffect = false;
            }
        }
    }
}

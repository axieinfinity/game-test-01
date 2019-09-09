using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine.Events;
using Zenject;
using Random = UnityEngine.Random;

public class SimpleGroupAttackingLogicStrategy : IGroupAttackingLogicStrategy
{
    [Inject] private IFactory<IAttackingLogicStrategy> _attackingLogicFac;

    private IEnumerable<(Character view, CharacterModel model)> _attackers;
    private IEnumerable<(Character view, CharacterModel model)> _targets;
    private UnityAction _onCompleted;
    private bool[] _attackingStateList = new bool[0];


    public IGroupAttackingLogicStrategy SetAttackers(IEnumerable<(Character view, CharacterModel model)> attackers)
    {
        _attackers = attackers;
        return this;
    }

    public IGroupAttackingLogicStrategy SetTarget(IEnumerable<(Character view, CharacterModel model)> targets)
    {
        _targets = targets;
        return this;
    }

    public IGroupAttackingLogicStrategy OnCompleted(UnityAction onCompleted)
    {
        _onCompleted = onCompleted;
        return this;
    }

    public void Attack()
    {
        _attackingStateList = new bool[_attackers.Count()];
        var count = 0;
        _attackers.ToObservable()
            .Subscribe(attacker =>
            {
                var i = count;
                _attackingStateList[i] = true;
                //random an enemy
                var target = _targets.ElementAt(Random.Range(0,
                    _targets.Count()));

                _attackingLogicFac.Create()
                    .SetAttackerView(attacker.view)
                    .SetAttackerModel(attacker.model)
                    .SetTargetView(target.view)
                    .SetTargetModel(target.model)
                    .OnCompleted(() => _attackingStateList[i] = false)
                    .Attack();
                count++;
            });
    }


    public bool IsAttacking()
    {
        return _attackingStateList.Length > 0 && _attackingStateList.Any(b => b);
    }
}

public interface IGroupAttackingLogicStrategy
{
    IGroupAttackingLogicStrategy SetAttackers(IEnumerable<(Character view, CharacterModel model)> attackers);
    IGroupAttackingLogicStrategy SetTarget(IEnumerable<(Character view, CharacterModel model)> targets);
    IGroupAttackingLogicStrategy OnCompleted(UnityAction onCompleted);
    void Attack();

    bool IsAttacking();
}
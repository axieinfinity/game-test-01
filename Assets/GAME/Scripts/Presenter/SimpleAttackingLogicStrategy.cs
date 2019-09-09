using UnityEngine.Events;

public interface IAttackingLogicStrategy
{
    IAttackingLogicStrategy SetAttackerView(Character attacker);

    IAttackingLogicStrategy SetTargetView(Character target);

    IAttackingLogicStrategy SetAttackerModel(CharacterModel attacker);

    IAttackingLogicStrategy SetTargetModel(CharacterModel target);

    IAttackingLogicStrategy OnCompleted(UnityAction action);

    void Attack();
}
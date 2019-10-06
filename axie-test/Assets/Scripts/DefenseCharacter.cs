
using System;

public class DefenseCharacter : Character
{
    public override void BehaveOnUserInput(System.Action<int> completeBehaviorCallback)
    {
        if (state != CHARACTER_STATE.ACTIVE)
        {
            throw new Exception("character state inactive");
            // return;
        }

        var enemy = GridController.instance.GetAdjacentEnemy(gridPosition, CharacterModel.CHARACTER_TYPE.ATTACK);
        if (enemy != null)
        {
            Attack(enemy, completeBehaviorCallback);
        }
        else
        {
            // SetAnimOnClick();
            completeBehaviorCallback(1);
        }
        // SetAnimOnClick();
    }
}
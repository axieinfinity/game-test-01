
using System;

public class DefenseCharacter : Character
{
    public override void BehaveOnUserInput()
    {
        // var enemy = GridController.instance.GetAdjacentEnemy(gridPosition, CharacterModel.CHARACTER_TYPE.ATTACK);
        // if (enemy != null)
        // {
        //     Attack(enemy);
        // }
        // else
        // {
        //     SetAnimOnClick();a
        // }
        SetAnimOnClick();
    }

    // private void Attack(Character enemy)
    // {
    //     this.Log("attacking");
    //     SetAnimAttack();
    //     var randomPoint = Random.Range(0, 2);
    //     enemy.GetComponent<IHitable>().GetHit(randomPoint);
    //     canClick = true;
    // }
}
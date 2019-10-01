using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class AttackCharacter : Character
{
    public override void BehaveOnUserInput()
    {
        var enemy = GridController.instance.GetAdjacentEnemy(gridPosition, CharacterModel.CHARACTER_TYPE.DEFENSE);
        if (enemy != null)
        {
            var enemyPosition = enemy.gridPosition;
            var enemyCell = GridController.instance.GetCell(enemyPosition);
            enemyCell.HighLight();
            Attack(enemy);
        }
        else
        {
            enemy = GameController.instance.FindClosestEnemyToMove(transform.position);
            if (enemy != null)
            {
                // this.Log(enemy);
                var targetCell = GridController.instance.GetAdjacentPosition(enemy.gridPosition);
                if (targetCell != null)
                {
                    GridController.instance.RemoveCharacterFromCell(gridPosition);
                    targetCell.character = this;
                    targetCell.HighLight();
                    this.gridPosition = targetCell.gridPosition;
                    this.name = "character[" + gridPosition.x + "." + gridPosition.y + "]";
                    MoveTo(targetCell);
                }
            }
            else
            {
                canClick = true;
                // StayIdle();
                SetAnimOnClick();
            }
        }
    }

    private void MoveTo(CellController targetCell)
    {
        SetAnimMove();
        transform.DOMove(targetCell.transform.position, 1.5f).OnComplete(() =>
        {
            canClick = true;
            SetAnimIdle();
        });
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
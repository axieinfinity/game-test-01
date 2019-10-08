using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class AttackCharacter : Character
{
    public override void BehaveOnUserInput(System.Action<int> completeBehaviorCallback)
    {
        if (state != CHARACTER_STATE.ACTIVE)
        {
            throw new Exception("character state inactive");
            // return;
        }

        var enemy = GridController.instance.GetAdjacentEnemy(gridPosition, CharacterModel.CHARACTER_TYPE.DEFENSE);
        if (enemy != null)
        {
            var enemyPosition = enemy.gridPosition;
            var enemyCell = GridController.instance.GetCell(enemyPosition);
            Attack(enemy, completeBehaviorCallback);
        }
        else
        {
            var closestEnemy = GameController.instance.FindClosestEnemy(transform.position, circleIndex);
            if (closestEnemy != null)
            {
                var list = GridController.instance.GetAdjacentCells(gridPosition);
                var targetCell = GetClosestCellToEnemy(list, closestEnemy);

                if (targetCell != null)
                {
                    GridController.instance.RemoveCharacterFromCell(gridPosition);
                    targetCell.character = this;
                    this.gridPosition = targetCell.gridPosition;
                    this.name = "character[" + gridPosition.x + "." + gridPosition.y + "]";
                    MoveTo(targetCell, completeBehaviorCallback);
                }
                else
                {
                    canClick = true;
                    completeBehaviorCallback(1);
                }
            }
            else
            {
                completeBehaviorCallback(1);
            }
        }
    }

    private CellController GetClosestCellToEnemy(List<CellController> list, Character enemy)
    {
        float min = float.MaxValue;
        CellController result = null;

        for (int i = 0; i < list.Count; i++)
        {
            var ele = list[i];
            var distance = Vector2.Distance(ele.transform.position, enemy.transform.position);
            if (distance < min && ele.character == null)
            {
                min = distance;
                result = ele;
            }
        }
        return result;
    }

    private void MoveTo(CellController targetCell, Action<int> callback)
    {
        SetAnimMove();
        transform.DOMove(targetCell.transform.position, 1.5f).OnComplete(() =>
        {
            canClick = true;
            SetAnimIdle();
            callback(1);
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
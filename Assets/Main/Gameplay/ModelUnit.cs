using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public partial class Model : MonoBehaviour
	{

        [Header("Unit setting")]
        [SerializeField] private int unitTargetScanRange = 1;

        void PlayUnitTurn(Unit unit)
		{
			if (unit.attacker == false) return;
			if (unit.dead) return;

            var currentTile = mapUnitToTile[unit];
            var targetTile = centerTile;

            var targetUnit = GetNearestTargetUnit(unit);
            if (targetUnit != null) targetTile = mapUnitToTile[targetUnit];
            
            if (targetUnit != null && Distance(mapUnitToTile[targetUnit], currentTile) <= 1)
                PlayUnitCombat(unit, targetUnit);
            else
                MoveUnitToTile(unit, GetNextFreeTileTowardTile(currentTile, targetTile));
		}

        Unit GetNearestTargetUnit(Unit unit)
        {
            var unitTile = mapUnitToTile[unit];
            foreach (var tile in IterateFromTile(unitTile, unitTargetScanRange))
                if (mapTileToUnit.ContainsKey(tile))
                {
                    var possibleTarget = mapTileToUnit[tile];
                    if (possibleTarget.dead) continue;
                    if (possibleTarget.attacker != unit.attacker) 
                        return possibleTarget;
                }
            return null;
        }

        void MoveUnitToTile(Unit unit, Tile targetTile)
        {
            if (mapTileToUnit.ContainsKey(targetTile)) return;

            var currentTile = mapUnitToTile[unit];
            mapTileToUnit[targetTile] = unit;
            mapTileToUnit.Remove(currentTile);
            mapUnitToTile[unit] = targetTile;
            unit.tileIndex = targetTile.index;

            onGameplayEvent?.Invoke(new EventUnitMoveToTile()
            {
                unitIndex = unit.index,
                tileIndex = targetTile.index
            });
        }

        void PlayUnitCombat(Unit attacker, Unit defender)
        {
            if (attacker.dead || defender.dead) return;
            onGameplayEvent?.Invoke(MakeAttack(attacker, defender));

            if (defender.countered) return;
            defender.countered = true;
            var eventCounterAttack = MakeAttack(defender, attacker);
            eventCounterAttack.counterAttack = true;
            onGameplayEvent?.Invoke(eventCounterAttack);
        }

        int[] damageResults = { 4, 5, 3 };
        EventUnitAttackUnit MakeAttack(Unit source, Unit target)
        {
            var sourceValue = UnityEngine.Random.Range(0, 3);
            var targetValue = UnityEngine.Random.Range(0, 3);
            var damage = damageResults[(3 + sourceValue - targetValue) % 3];

            target.hp -= damage;
            if (target.hp <= 0)
            {
                target.dead = true;
                target.hp = 0;
            }

            return new EventUnitAttackUnit()
            {
                sourceIndex = source.index,
                targetIndex = target.index,
                remainingHp = target.hp,
                damage = damage,
            };
        }

        void RemoveDeadUnit(Unit unit)
        {
            if (unit.dead == false || unit.tileIndex < 0) return;
            if (mapUnitToTile.ContainsKey(unit) == false) return;

            unit.tileIndex = -1;
            var tile = mapUnitToTile[unit];
            mapUnitToTile.Remove(unit);
            mapTileToUnit.Remove(tile);

            onGameplayEvent?.Invoke(new EventUnitDead()
            {
                index = unit.index
            });
        }
	}
}

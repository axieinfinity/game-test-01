using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
	public partial class Model : MonoBehaviour
	{

        [Header("Unit setting")]
        public int unitTargetScanRange = 4;

        void PlayUnitTurn(Unit unit)
		{
			if (unit.attacker == false) return;
			if (unit.dead) return;

            var currentTile = mapUnitToTile[unit];
            var targetTile = centerTile;

            var targetUnit = GetNearestTargetUnit(unit);
            if (targetUnit != null) targetTile = mapUnitToTile[targetUnit];
            
            var nextTile = GetNextFreeTileTowardTile(currentTile, targetTile);

            if (mapTileToUnit.ContainsKey(nextTile)) return;
            mapTileToUnit[nextTile] = unit;
            mapTileToUnit.Remove(currentTile);
            mapUnitToTile[unit] = nextTile;
            unit.tileIndex = nextTile.index;

            onGameplayEvent?.Invoke(new EventUnitMoveToTile()
            {
                unitIndex = unit.index,
                tileIndex = nextTile.index
            });
		}

        Unit GetNearestTargetUnit(Unit unit)
        {
            var unitTile = mapUnitToTile[unit];
            foreach (var tile in IterateFromTile(unitTile, unitTargetScanRange))
                if (mapTileToUnit.ContainsKey(tile))
                {
                    var possibleTarget = mapTileToUnit[tile];
                    if (possibleTarget.attacker != unit.attacker) 
                        return mapTileToUnit[tile];
                }
            return null;
        }

        void RemoveDeadUnit(Unit unit)
        {
            //DO SOME CLEANUP HERE
        }
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gameplay
{
    public partial class Model : MonoBehaviour
    {

        public OnReportUpdated onReportUpdated;
        [System.Serializable] public class OnReportUpdated : UnityEngine.Events.UnityEvent<Report> { }

        private Report report;
        public struct Report
        {
            public int attackerCount, defenderCount;
            public int attackerDamageLastTurn, defenderDamageLastTurn;
            public int attackerTotalHp, defenderTotalHp;
            public int turnElapsed;
        }

        void SetupReport()
        {
            this.onGameplayEvent.AddListener(UpdateReportCombat);
        }

        void UpdateReportCombat(Event e)
        {
            if ((e is EventUnitAttackUnit) == false) return;
            var attackEvent = e as EventUnitAttackUnit;

            var source = units[attackEvent.sourceIndex];
            if (source.attacker)
                report.attackerDamageLastTurn += attackEvent.damage;
            else
                report.defenderDamageLastTurn += attackEvent.damage;
        }

        void UpdateReport()
        {
            report.attackerCount = report.defenderCount = report.attackerTotalHp = report.defenderTotalHp = 0;

            report.turnElapsed++;
            foreach (var unit in units)
            {
                if (unit.dead) continue;
                if (unit.attacker)
                {
                    report.attackerCount++;
                    report.attackerTotalHp += unit.hp;
                }
                else
                {
                    report.defenderCount++;
                    report.defenderTotalHp += unit.hp;
                }
            }

            onReportUpdated?.Invoke(report);
            report.attackerDamageLastTurn = report.defenderDamageLastTurn = 0;
        }

    }
}

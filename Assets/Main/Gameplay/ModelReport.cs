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
        }

        void UpdateReport()
        {

            onReportUpdated?.Invoke(report);
            report.attackerDamageLastTurn = report.defenderDamageLastTurn = 0;
        }

    }
}

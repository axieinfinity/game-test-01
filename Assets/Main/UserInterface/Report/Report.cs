using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class Report : MonoBehaviour
    {

        public FPSCounter fpsCounter;

        [Space(32)]
        public Text textFPS;

        private void Awake()
        {
            groupBenchmark.SetActive(false);
            groupBenchmarkCompleted.SetActive(false);
        }

        private void Update()
        {
            textFPS.text = (Mathf.RoundToInt(fpsCounter.FPS)).ToString();
        }

		public GameObject groupBenchmark;
		public Text textGroupBenchmarkCount;

		public void ReportBenchmarking(int ringCount)
		{
            groupBenchmark.gameObject.SetActive(true);
            textGroupBenchmarkCount.text = ringCount.ToString();
        }

		public GameObject groupBenchmarkCompleted;
		public Text textGroupBenchmarkCompletedCount;

		public void ReportBenchmarkingCompleted(int ringCount)
        {
            groupBenchmark.gameObject.SetActive(false);
            groupBenchmarkCompleted.gameObject.SetActive(true);
            textGroupBenchmarkCompletedCount.text = ringCount.ToString();
        }

        [Space(32)]
        public Text textTurnElapsed;
        public Text textAttackerUnitCount, textDefenderUnitCount;
        public Text textAttackerDamage, textDefenderDamage;
        public Text textAttackerTotalHp, textDefenderTotalHp;

        public void UpdateReport(Model.Report report)
        {
            textTurnElapsed.text = report.turnElapsed.ToString();
            textAttackerUnitCount.text = report.attackerCount.ToString();
            textDefenderUnitCount.text = report.defenderCount.ToString();

            textAttackerDamage.text = report.attackerDamageLastTurn.ToString();
            textDefenderDamage.text = report.defenderDamageLastTurn.ToString();

            textAttackerTotalHp.text = report.attackerTotalHp.ToString();
            textDefenderTotalHp.text = report.defenderTotalHp.ToString();
        }

    }
}
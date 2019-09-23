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

        private void Update()
        {
            textFPS.text = (Mathf.RoundToInt(fpsCounter.FPS)).ToString();
        }

        public void UpdateReport(Model.Report report)
        {

        }

    }
}
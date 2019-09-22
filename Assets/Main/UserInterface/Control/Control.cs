using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class Control : MonoBehaviour
    {
        public Text text;
        private int speedRate = 1;
        private bool pausing = false;

        // Start is called before the first frame update
        void Start()
        {
            text.text = "x1";
        }

        public void FunctionSetPaused(bool pausing)
        {
            this.pausing = pausing;
            CalculateSpeed();
        }

        public void FunctionSetSpeedRate(float speedRate)
        {
            int rate = Mathf.RoundToInt(Mathf.Pow(2, speedRate));
            this.speedRate = rate;
            
            text.text = "x" + rate;
            CalculateSpeed();
        }

        void CalculateSpeed()
        {
            Time.timeScale = pausing ? 0 : speedRate;
        }
    }
}
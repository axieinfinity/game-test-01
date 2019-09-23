using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public static class Settings
    {
        public static bool enabled;
        public static int ringCount = 1;
        public static bool benchmarkMode;
        public static float minBenchmarkFPS = 30;
    }
}

namespace Gameplay.UI {
    public class MainMenu : MonoBehaviour
    {

        public InputField inputRingCount;

        private void Awake()
        {
            Settings.enabled = true;
        }

        public void ValidateRingCount(string text)
        {
            int value = int.Parse(text);
            if (value <= 0) value = 1;

            inputRingCount.text = value.ToString();
            Settings.ringCount = value;
        }

        public void FunctionStart()
        {
            ValidateRingCount(inputRingCount.text);
            Settings.benchmarkMode = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Main");
        }

        public void FunctionBenchmark()
        {
            ValidateRingCount(inputRingCount.text);
            Settings.ringCount = 1;
            Settings.benchmarkMode = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Main");
        }
    }
}
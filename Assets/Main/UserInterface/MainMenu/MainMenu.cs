using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
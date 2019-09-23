using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class FPSCounter : MonoBehaviour
    {
        private float deltaTime = 1 / 60f;
        public float FPS { get; private set; }

        // Update is called once per frame
        void Update()
        {
            deltaTime = (deltaTime + Time.unscaledDeltaTime) / 2;
            FPS = 1 / deltaTime;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class MinimapCamera : MonoBehaviour
    {

        public void SetViewLimit(Rect viewLimit)
        {
            var cam = GetComponent<Camera>();
            if (cam == null) return;

            cam.orthographicSize = viewLimit.height / 2 + 2;
        }
    }
}
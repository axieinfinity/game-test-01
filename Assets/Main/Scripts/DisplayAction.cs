using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class DisplayAction : ScriptableObject
    {
        public string actionSource, actionTarget;
        public float delaySourceToTarget;
    }
}
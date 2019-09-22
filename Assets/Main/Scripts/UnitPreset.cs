using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class UnitPreset : ScriptableObject
    {
        public string unitName;
        public int hp;
        public Spine.Unity.SkeletonDataAsset skeleton;
    }
}
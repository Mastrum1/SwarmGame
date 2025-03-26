using System;
using UnityEngine;

namespace Common.ScreenShake
{
    [Serializable]
    public class ScreenShakeParams
    {
        public float Strength;
        public float Duration;
        public AnimationCurve Curve;
    }
}
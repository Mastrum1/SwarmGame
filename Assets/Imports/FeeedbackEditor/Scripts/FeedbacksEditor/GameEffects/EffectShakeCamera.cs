using System.Collections;
using Common.ScreenShake;
using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// Shakes the main camera at a certain force for a certain time.
    /// </summary>
    [System.Serializable]
    public class EffectShakeCamera : GameEffect
    {
        public ScreenShakeParams ScreenShakeParams;
        
        public override IEnumerator Execute(GameEvent gameEvent, GameObject target)
        {
            ScreenShake.Shake(ScreenShakeParams);
            yield break;
        }

        public override Color GetColor() => new Color(.7f, .5f, .3f);

        public override string ToString()
        {
            return $"Camera Shake Force {ScreenShakeParams.Strength} during {ScreenShakeParams.Duration} seconds";
        }
    }
}
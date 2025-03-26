using System;
using System.Collections;
using Common.TimeScale;
using UnityEngine;

namespace FeedbacksEditor
{
    [Serializable]
    public class EffectSlowMotion : GameEffect
    {
        public TimeScaleParams TimeScaleParams;
        public override IEnumerator Execute(GameEvent gameEvent, GameObject target)
        {
            TimeScaleManager.ChangeTimeScale(TimeScaleParams);
            yield return new WaitForSecondsRealtime(TimeScaleParams.Duration);
        }

        public override Color GetColor() => new Color(0.3f, 0.3f, 0.7f);

        public override string ToString()
        {
            return $"Slow Motion for {TimeScaleParams.Duration} seconds";
        }
    }
}
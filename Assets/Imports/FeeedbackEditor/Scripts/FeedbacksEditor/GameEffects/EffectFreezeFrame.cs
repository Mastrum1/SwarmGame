using System;
using System.Collections;
using Common.TimeScale;
using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// Freezes the time for a certain time.
    /// </summary>
    [Serializable]
    public class EffectFreezeFrame : GameEffect
    {
        public float Timer;
        
        public override IEnumerator Execute(GameEvent gameEvent, GameObject target)
        {
            //var oldTimeScale = Time.timeScale;
            Time.timeScale = 0;
            
            yield return new WaitForSecondsRealtime(Timer);
            
            //Time.timeScale = oldTimeScale;
            Time.timeScale = 1;
        }

        public override Color GetColor() => new Color(0.3f, 0.5f, 0.7f);

        public override string ToString()
        {
            return $"Freeze Frame for {Timer} seconds";
        }
    }
}
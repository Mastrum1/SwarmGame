using System;
using System.Collections;
using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// Waits for a durable before calling the next instruction.
    /// </summary>
    [Serializable]
    public class EffectWait : GameEffect
    {
        public float Timer;
        
        public override IEnumerator Execute(GameEvent gameEvent, GameObject target)
        {
            yield return new WaitForSeconds(Timer);
        }
        
        public override Color GetColor() => new Color(0.3f, .7f, 0.3f);
        
        public override string ToString()
        {
            return $"Waits for {Timer} seconds";
        }
    }
}
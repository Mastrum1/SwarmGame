using System;
using System.Collections;
using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// The parent class for all gameEffects.
    /// </summary>
    [Serializable]
    public abstract class GameEffect
    {
        public bool Enabled = true;
        public abstract IEnumerator Execute(GameEvent gameEvent, GameObject target);

        public abstract Color GetColor();
    }
}
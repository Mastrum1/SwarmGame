using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// A Game Event, containing multiple effects in a sequence.
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "GameEvent", menuName = "Feedback Editor/GameEvent")]
    public class GameEvent : ScriptableObject
    {
        [SerializeReference]
        public List<GameEffect> Effects = new List<GameEffect>();

        public IEnumerator Execute(GameObject target)
        {
            foreach (var effect in Effects)
            {
                if (effect.Enabled) 
                    yield return effect.Execute(this, target);
            }
        }
    }
}
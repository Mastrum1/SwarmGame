using AntoineFoucault.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    [Serializable]
    public class EffectSound : GameEffect
    {
        public List<AudioClip> Clips = new List<AudioClip>();
        public override IEnumerator Execute(GameEvent gameEvent, GameObject target)
        {
            var clip = Clips.GetRandomItem();
            AudioManager.Instance?.PlayClip(clip);
            yield break;
        }

        public override Color GetColor() => new Color(.7f,.4f,.7f);

        public override string ToString()
        {
            return $"Play Sound {((Clips.Count < 1 || Clips[0] == null) ? "null" : Clips[0].name + $" and {Clips.Count - 1} others")}";
        }
    }
}
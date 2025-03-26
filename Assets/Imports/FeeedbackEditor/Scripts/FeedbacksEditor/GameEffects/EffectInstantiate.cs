using System;
using System.Collections;
using UnityEngine;

namespace FeedbacksEditor
{
    /// <summary>
    /// Instantiates a prefab at the target's position.
    /// </summary>
    [Serializable]
    public class EffectInstantiate : GameEffect
    {
        public GameObject Prefab;

        public override IEnumerator Execute(GameEvent gameEvent, GameObject target)
        {
            if (Prefab != null)
            {
                if (target == null)
                    GameObject.Instantiate(Prefab, null);
                else
                    GameObject.Instantiate(Prefab, target.transform.position, Quaternion.identity);
            }
            else Debug.LogWarning("Prefab of EffectInstantiate is null");
            yield break;
        }

        public override Color GetColor() => new Color(.8f, .8f, .5f);

        public override string ToString()
        {
            return $"Instantiate {((Prefab == null) ? "null" : Prefab.name)}";
        }
    }
}
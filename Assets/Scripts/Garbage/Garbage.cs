using FeedbacksEditor;
using UnityEngine;

namespace Garbage
{
    /// <summary>
    /// A piece of garbage on the ground
    /// </summary>
    public class Garbage : MonoBehaviour
    {
        [field:SerializeField] public int EntitiesToAdd { get; private set; }
        [SerializeField] private GameEvent _feedback;
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out GarbageCleaner cleaner) == false) return;
            GameEventsManager.PlayEvent(_feedback, gameObject);
            cleaner.Clean(this);
        }
    }
}
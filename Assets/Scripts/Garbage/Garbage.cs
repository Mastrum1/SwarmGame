using System;
using FeedbacksEditor;
using UnityEngine;

namespace Garbage
{
    /// <summary>
    /// A piece of garbage on the ground
    /// </summary>
    public class Garbage : MonoBehaviour
    {
        public bool IsRotable { get => _isRotable; private set => _isRotable = value;}
        [field:SerializeField] public int EntitiesToAdd { get; private set; }
        
        [SerializeField] private GameEvent _feedback;
        
        [SerializeField] private bool _isRotable;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out GarbageCleaner cleaner) == false) return;
            GameEventsManager.PlayEvent(_feedback, gameObject);
            cleaner.Clean(this);
        }
    }
}
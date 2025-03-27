using System;
using FeedbacksEditor;
using Garbage.Plants;
using UnityEngine;

namespace Garbage
{
    /// <summary>
    /// A piece of garbage on the ground
    /// </summary>
    public class Garbage : MonoBehaviour
    {
        private static PlantSpawner _plantSpawner;

        public bool IsRotable { get => _isRotable; private set => _isRotable = value;}
        [field:SerializeField] public int EntitiesToAdd { get; private set; }
        
        [SerializeField] private GameEvent _feedback;
        
        [SerializeField] private bool _isRotable;

        private void Awake()
        {
            if (_plantSpawner == null)
            {
                _plantSpawner = GameObject.FindAnyObjectByType<PlantSpawner>();
                if (_plantSpawner == null)
                    Debug.LogError("No PlantSpawner found in the scene");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out GarbageCleaner cleaner) == false) return;
            GameEventsManager.PlayEvent(_feedback, gameObject);

            if (_plantSpawner != null)
                _plantSpawner.Spawn(transform.position);

            cleaner.Clean(this);
        }
    }
}
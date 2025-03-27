using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Garbage
{
    /// <summary>
    /// Keeps track of the total percentage of the cleaning, both taking into account the garbage and the ground itself
    /// </summary>
    public class TotalCleaningManager : MonoBehaviour
    {
        public Action<TotalCleaningManager> OnPercentageChanged;
        public float CleanedPercentage => (float)CurrentEntities / _totalEntities;
        public int CurrentEntities { get; private set; }

        [SerializeField] private Transform _parent;
        [SerializeField] private Garbage[] _garbagePrefabList;
        [SerializeField] private int _garbageCount;

        [SerializeField] private List<Garbage> _totalGarbages = new();
        
        

        private int _notCleanedGarbage => _totalGarbages.Count;
        private int _totalEntities;

        private void Start()
        {
            
            AssignGarbages();
            var planet = MainGame.Instance.Planet;
            var radius = planet.radius * planet.transform.localScale.y;
            for (int i = 0; i < _garbageCount; i++)
            {
                CreateGarbage(_garbagePrefabList[Random.Range(0, _garbagePrefabList.Length)], UnityEngine.Random.onUnitSphere * radius);
            }

            MainGame.Instance.OnGarbageCleaned += OnGarbageCleaned;
        }

        public void CreateGarbage(Garbage prefab, Vector3 position)
        {
            var garbage = Instantiate(prefab, position, Quaternion.identity, _parent);
            var randomRotation = Random.rotation;

            if (garbage.IsRotable) garbage.transform.rotation = randomRotation;
            else
            {
                Vector3 direction = (MainGame.Instance.Planet.transform.position - transform.position).normalized;
                Vector3 forward = Vector3.Cross(direction, Random.onUnitSphere);
                garbage.transform.rotation = Quaternion.LookRotation(forward, direction);
            }
            
            _totalGarbages.Add(garbage);
            _totalEntities += garbage.EntitiesToAdd;
        }

        private void OnDisable()
        {
            MainGame.Instance.OnGarbageCleaned -= OnGarbageCleaned;
        }

        private void OnGarbageCleaned(Garbage garbage)
        {
            _totalGarbages.Remove(garbage);
            CurrentEntities += garbage.EntitiesToAdd;
            OnPercentageChanged?.Invoke(this);
        }

        private void Reset()
        {
            AssignGarbages();
        }

        /// <summary>
        /// Method to assign all garbages to the variable. Called only in the editor.
        /// </summary>
        public void AssignGarbages()
        {
            _totalGarbages = GameObject.FindObjectsByType<Garbage>(FindObjectsSortMode.None).ToList();
        }
    }
}
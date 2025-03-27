using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        [SerializeField] private Garbage _garbagePrefab;
        [SerializeField] private int _garbageCount;

        public List<Garbage> TotalGarbages = new();

        private int _notCleanedGarbage => TotalGarbages.Count;
        private int _totalEntities;

        private void Start()
        {
            
            AssignGarbages();
            var planet = MainGame.Instance.Planet;
            var radius = planet.radius * planet.transform.localScale.y;
            for (int i = 0; i < _garbageCount; i++)
            {
                CreateGarbage(_garbagePrefab, UnityEngine.Random.onUnitSphere * radius);
            }

            MainGame.Instance.OnGarbageCleaned += OnGarbageCleaned;
        }

        public void CreateGarbage(Garbage prefab, Vector3 position)
        {
            var garbage = Instantiate(prefab, position, UnityEngine.Quaternion.identity, _parent);
            TotalGarbages.Add(garbage);
            _totalEntities += garbage.EntitiesToAdd;
            garbage.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            MainGame.Instance.OnGarbageCleaned -= OnGarbageCleaned;
        }

        private void OnGarbageCleaned(Garbage garbage)
        {
            TotalGarbages.Remove(garbage);
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
            TotalGarbages = GameObject.FindObjectsByType<Garbage>(FindObjectsSortMode.None).ToList();
        }
    }
}
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Callbacks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Garbage
{
    /// <summary>
    /// Keeps track of the total percentage of the cleaning, both taking into account the garbage and the ground itself
    /// </summary>
    public class TotalCleaningManager : MonoBehaviour
    {
        public GameObject GarbagePrefab;
        public Transform Parent;
        public Action<float> OnPercentageChanged;
        public int GarbageCount;
        public float SphereDistance;
        public float Percentage => (float)_cleanedGarbage / _totalGarbage;

        [SerializeField] private List<Garbage> _totalGarbages = new();

        private int _totalGarbage;
        private int _cleanedGarbage;

        private void Awake()
        {
            //AssignGarbages();
            for (int i = 0; i < GarbageCount; i++)
            {
                Garbage g = Instantiate(GarbagePrefab, Random.onUnitSphere * SphereDistance, Quaternion.identity, Parent).GetComponent<Garbage>();
                _totalGarbages.Add(g);

            }

        }

        private void Start()
        {
            
            MainGame.Instance.OnGarbageCleaned += OnGarbageCleaned;

        }

        private void OnDisable()
        {
            MainGame.Instance.OnGarbageCleaned -= OnGarbageCleaned;
        }

        private void OnGarbageCleaned(Garbage garbage)
        {
            _cleanedGarbage += garbage.EntitiesToAdd;
            OnPercentageChanged?.Invoke(Percentage);
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
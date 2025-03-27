using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;

namespace Garbage
{
    /// <summary>
    /// Keeps track of the total percentage of the cleaning, both taking into account the garbage and the ground itself
    /// </summary>
    public class TotalCleaningManager : MonoBehaviour
    {
        public Action<TotalCleaningManager> OnPercentageChanged;
        public float CleanedPercentage => CleanedGarbageCount / GarbageCount;


        public Transform Parent;
        public GameObject GarbagePrefab;
        public int GarbageCount;
        public float SpawnRadius;
        [SerializeField] private List<Garbage> _totalGarbages = new();

        public int NotCleanedGarbageCount => _totalGarbages.Count;
        public int CleanedGarbageCount => GarbageCount - NotCleanedGarbageCount;

        private void Awake()
        {
            AssignGarbages();
            _totalGarbages.Add(
                Instantiate(GarbagePrefab, 
                            UnityEngine.Random.onUnitSphere * SpawnRadius, 
                            UnityEngine.Quaternion.identity, 
                            Parent)
                .GetComponent<Garbage>()
            );
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
            _totalGarbages.Remove(garbage);
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
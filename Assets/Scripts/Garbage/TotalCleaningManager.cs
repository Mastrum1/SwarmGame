using System;
using UnityEngine;

namespace Garbage
{
    /// <summary>
    /// Keeps track of the total percentage of the cleaning, both taking into account the garbage and the ground itself
    /// </summary>
    public class TotalCleaningManager : MonoBehaviour
    {
        public Action<float> OnPercentageChanged;
        public float Percentage => (float)_cleanedGarbage / _totalGarbage;
        
        [SerializeField] private Garbage[] _totalGarbages;

        private int _totalGarbage;
        private int _cleanedGarbage;

        private void Start()
        {
            foreach (var garbage in _totalGarbages)
            {
                _totalGarbage += garbage.EntitiesToAdd;
            }

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
            _totalGarbages = GameObject.FindObjectsByType<Garbage>(FindObjectsSortMode.None);
        }
    }
}
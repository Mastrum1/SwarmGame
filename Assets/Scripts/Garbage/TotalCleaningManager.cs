using System;
using System.Collections.Generic;
using System.Linq;
using AntoineFoucault.Utilities;
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

        private List<GameObject> _totalGarbages = new();
        
        private GameObjectsActivator.VisibilityGroup _visibilityGroup;
        
        //private int _notCleanedGarbage => _totalGarbages.Count;
        private int _totalEntities;

        private void Start()
        {
            
            AssignGarbages();
            var planet = MainGame.Instance.Planet;
            var radius = planet.radius * planet.transform.localScale.y;
            for (int i = 0; i < _garbageCount; i++)
            {
                CreateGarbage(_garbagePrefabList.GetRandomItem().gameObject, UnityEngine.Random.onUnitSphere * radius);
            }

            MainGame.Instance.OnGarbageCleaned += OnGarbageCleaned;
            
            _visibilityGroup = new GameObjectsActivator.VisibilityGroup
            {
                objects = _totalGarbages,
                distanceToActivate = 55,
                distanceToDeactivate = 55
            };
            
            FindAnyObjectByType<GameObjectsActivator>().RegisterVisibilityGroup(_visibilityGroup);
        }

        public void CreateGarbage(GameObject prefab, Vector3 position)
        {
            if(!prefab.TryGetComponent(out Garbage _))
            {
                Debug.LogError("The prefab does not have a Garbage component");
                return;
            }
            
            var garbage = Instantiate(prefab, position, Quaternion.identity, _parent);
            Garbage garbageComp = garbage.GetComponent<Garbage>();
            var randomRotation = Random.rotation;
    
            if (garbageComp.IsRotable) garbage.transform.rotation = randomRotation;
            else
            {
                Vector3 direction = (MainGame.Instance.Planet.transform.position - transform.position).normalized;
                Vector3 forward = Vector3.Cross(direction, Random.onUnitSphere).normalized;
                if (forward.magnitude < 0.01f)
                    forward = garbage.transform.forward;

                garbage.transform.rotation = Quaternion.LookRotation(forward, direction);
            }
            
            _totalGarbages.Add(garbage);
            _totalEntities += garbage.GetComponent<Garbage>().EntitiesToAdd;
        }

        private void OnDisable()
        {
            MainGame.Instance.OnGarbageCleaned -= OnGarbageCleaned;
            FindAnyObjectByType<GameObjectsActivator>()?.UnregisterVisibilityGroup(_visibilityGroup);
        }

        private void OnGarbageCleaned(Garbage garbage)
        {
            _totalGarbages.Remove(garbage.gameObject);
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
            _totalGarbages = GameObject.FindObjectsByType<Garbage>(FindObjectsSortMode.None).Select(i => i.gameObject).ToList();
        }
    }
}
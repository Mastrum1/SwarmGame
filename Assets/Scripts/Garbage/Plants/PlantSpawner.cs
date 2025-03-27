using AntoineFoucault.Utilities;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Garbage.Plants
{
    /// <summary>
    /// Spawns a plant on destroy
    /// </summary>
    public class PlantSpawner : MonoBehaviour
    {
        [SerializeField] private PlantsPrefabsFactory _factory;

        private List<GameObject> _plants = new();
        private GameObjectsActivator.VisibilityGroup _visibilityGroup;

        private GameObjectsActivator _activater;

        private void Awake()
        {
            _visibilityGroup = new GameObjectsActivator.VisibilityGroup
            {
                objects = _plants,
                distanceToActivate = 55,
                distanceToDeactivate = 55
            };
            _activater = FindAnyObjectByType<GameObjectsActivator>();
            
            if(_activater == null)
            {
                Debug.LogError("No GameObjectsActivator found in the scene");
                return;
            }

        }
        private void Start()
        {
            _activater.RegisterVisibilityGroup(_visibilityGroup);

        }
        private void OnDestroy()
        {
            if(_activater != null)
            {
                _activater.UnregisterVisibilityGroup(_visibilityGroup);
            }
        }

        public void Spawn(GameObject origion)
        {
            Vector3 direction = (MainGame.Instance.Planet.transform.position - transform.position).normalized;
            Vector3 forward = Vector3.Cross(direction, Random.onUnitSphere).normalized;
            if (forward.magnitude < 0.01f)
                forward = origion.transform.forward;

            Quaternion rotation = Quaternion.LookRotation(forward, direction); 
            
            var randomPlant = _factory.PlantPrefabs.GetRandomItem();
            var newPlant = Instantiate(randomPlant, origion.transform.position, rotation, transform);
            _plants.Add(newPlant);

            
        }
    }
}
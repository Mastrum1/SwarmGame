using AntoineFoucault.Utilities;
using UnityEngine;

namespace Garbage.Plants
{
    /// <summary>
    /// Spawns a plant on destroy
    /// </summary>
    public class PlantSpawner : MonoBehaviour
    {
        [SerializeField] private PlantsPrefabsFactory _factory;

        private void OnDestroy()
        {
            var randomPlant = _factory.PlantPrefabs.GetRandomItem();
            var newPlant = Instantiate(randomPlant, transform.position, transform.rotation, null);
            
            Vector3 direction = (transform.position - MainGame.Instance.Planet.transform.position).normalized;
            Vector3 forward = Vector3.Cross(direction, Random.onUnitSphere);
            newPlant.transform.rotation = Quaternion.LookRotation(forward, direction);
        }
    }
}
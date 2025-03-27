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
            var newPlant = Instantiate(randomPlant, transform.position, transform.rotation, transform.parent);
            newPlant.transform.up = transform.position - (MainGame.Instance.Planet.transform.position);
        }
    }
}
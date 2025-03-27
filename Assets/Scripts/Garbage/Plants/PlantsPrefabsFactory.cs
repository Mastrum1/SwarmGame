using UnityEngine;

namespace Garbage.Plants
{
    [CreateAssetMenu(fileName = "Plants Prefabs")]
    public class PlantsPrefabsFactory : ScriptableObject
    {
        [field: SerializeField] public GameObject[] PlantPrefabs { get; private set; }
    }
}
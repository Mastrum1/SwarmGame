using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// References all of the entities and handles their creation and destruction
/// </summary>
public class SmallEntitiesManager : MonoBehaviour
{
    [field:SerializeField] public List<SmallEntity> Entities {get; private set;}
    [SerializeField] private SmallEntity _entityPrefab;

    private void Start()
    {
        MainGame.Instance.OnGarbageCleaned += AddEntities;
    }

    private void OnDisable()
    {
        MainGame.Instance.OnGarbageCleaned -= AddEntities;
    }

    private void AddEntities(Garbage.Garbage garbage)
    {
        for (int i = 0; i < garbage.EntitiesToAdd; i++)
        {
            SpawnEntity();
        }
    }

    private void SpawnEntity()
    {
        Instantiate(_entityPrefab, transform);
    }
}

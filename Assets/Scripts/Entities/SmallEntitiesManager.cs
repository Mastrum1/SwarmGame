using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// References all of the entities and handles their creation and destruction
/// </summary>
public class SmallEntitiesManager : MonoBehaviour
{
    [SerializeField] private SmallEntity _entityPrefab;
    [SerializeField] private BeesManager _beesManager;

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
        _beesManager.ActivateBee();
    }
}

using System.Collections.Generic;
using AntoineFoucault.Utilities;
using UnityEngine;


public class AddObjectsAroundPlanet : MonoBehaviour
{
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private int _amount;
    [SerializeField] private Transform _parent;
    
    [Header("Visibility Group")]
    [SerializeField] private GameObjectsActivator.VisibilityGroup _visibilityGroup;
    [SerializeField] private float _distanceToActivate = 55;
    [SerializeField] private float _distanceToDeactivate = 55;

    private void Start()
    {
        var planet = MainGame.Instance.Planet;
        var radius = planet.radius * planet.transform.localScale.y;
        var newGameObjects = new List<GameObject>();
        for (int i = 0; i < _amount; i++)
        {
            var go = CreateObject(_prefabs.GetRandomItem().gameObject, UnityEngine.Random.onUnitSphere * radius);
            newGameObjects.Add(go);
        }
        
        _visibilityGroup = new GameObjectsActivator.VisibilityGroup
        {
            objects = newGameObjects,
            distanceToActivate = _distanceToActivate,
            distanceToDeactivate = _distanceToDeactivate
        };
    }

    private GameObject CreateObject(GameObject prefab, Vector3 position)
    {
        var newGo = GameObject.Instantiate(prefab, position, Quaternion.identity, _parent);
        
        Vector3 direction = (MainGame.Instance.Planet.transform.position - newGo.transform.position).normalized;
        Vector3 forward = Vector3.Cross(direction, Random.onUnitSphere);
        newGo.transform.rotation = Quaternion.LookRotation(forward, direction);

        return newGo;
    }
}
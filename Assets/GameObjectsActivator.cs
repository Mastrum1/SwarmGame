using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsActivator : MonoBehaviour
{
    [System.Serializable]
    public class VisibilityGroup
    {
        public List<GameObject> objects = new List<GameObject>();
        public float distanceToActivate = 10f;
        public float distanceToDeactivate = 15f;
    }

    public List<VisibilityGroup> VisibilityGroups = new List<VisibilityGroup>();

    public float _distanceToActivate = 10f;
    public float _distanceToDeactivate = 15f;
    public float CheckInterval = 1f;
    public Transform RefferenceTransform;

    private Coroutine _coroutine;
    private void Awake()
    {
        if(RefferenceTransform == null)
        {
            RefferenceTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }
    private void Start()
    {
        if (gameObject.activeInHierarchy)
            _coroutine = StartCoroutine(CheackGarbageVisability());
    }

    private IEnumerator CheackGarbageVisability()
    {
        while (true)
        {
            foreach (var group in VisibilityGroups)
            {
                foreach (var obj in group.objects)
                {
                    if (obj == null)
                        continue;

                    float distance = Vector3.Distance(RefferenceTransform.position, obj.transform.position);

                    // If the object is active and farther than the deactivate distance, deactivate it.
                    if (obj.activeSelf && distance > group.distanceToDeactivate)
                    {
                        obj.SetActive(false);
                    }
                    // If the object is inactive and closer than the activate distance, activate it.
                    else if (!obj.activeSelf && distance < group.distanceToActivate)
                    {
                        obj.SetActive(true);
                    }
                }
            }
            yield return new WaitForSeconds(CheckInterval);
        }
    }

    private void OnDestroy()
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);
    }

    public void RegisterVisibilityGroup(List<GameObject> objects, float distanceToActivate, float distanceToDeactivate)
    {
        if (_coroutine != null)
            StopCoroutine(_coroutine);


        VisibilityGroup newGroup = new VisibilityGroup
        {
            objects = objects,
            distanceToActivate = distanceToActivate,
            distanceToDeactivate = distanceToDeactivate
        };
        VisibilityGroups.Add(newGroup);

        if (gameObject.activeInHierarchy)
            _coroutine = StartCoroutine(CheackGarbageVisability());
    }
    public void RegisterVisibilityGroup(VisibilityGroup group)
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);

        VisibilityGroups.Add(group);

        if (gameObject.activeInHierarchy)
            _coroutine = StartCoroutine(CheackGarbageVisability());

    }
    public void UnregisterVisibilityGroup(VisibilityGroup group)
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);

        VisibilityGroups.Remove(group);

        if(gameObject.activeInHierarchy)
            _coroutine = StartCoroutine(CheackGarbageVisability());

    }

}

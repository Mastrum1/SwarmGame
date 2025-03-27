using Garbage;
using System;
using System.Collections;
using UnityEngine;

public class GarbageActivator : MonoBehaviour
{
    [SerializeField] private TotalCleaningManager _totalCleaningManager;
    public float _distanceToActivate = 10f;
    public float _distanceToDeactivate = 15f;
    public float _checkInterval = 1f;

    private void Awake()
    {
        StartCoroutine(CheackGarbageVisability());
    }

    private IEnumerator CheackGarbageVisability()
    {
        while (true)
        {
            foreach (var garbage in _totalCleaningManager.TotalGarbages)
            {
                if (garbage.gameObject.activeSelf)
                {
                    if (Vector3.Distance(garbage.transform.position, transform.position) > _distanceToDeactivate)
                    {
                        garbage.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (Vector3.Distance(garbage.transform.position, transform.position) < _distanceToActivate)
                    {
                        garbage.gameObject.SetActive(true);
                    }
                }
            }
            yield return new WaitForSeconds(_checkInterval);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

}

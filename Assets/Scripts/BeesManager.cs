using System;
using UnityEngine;

public class BeesManager : MonoBehaviour
{
    [SerializeField] private int maxBees;
    [SerializeField] private GameObject beePrefab;
    [SerializeField] private int maxBeePainters;
    [SerializeField] private GameObject beePainterPrefab;
    
    private SmallEntity[] _bees;
    private SmallEntity[] _ownedBees;
    private GameObject[] _beePainters;
    private int _ownerBeesCount;
    private bool _randomisePainterParent = false;

    private void Start()
    {
        _ownerBeesCount = 0;
        _bees = new SmallEntity[maxBees];
        _ownedBees = new SmallEntity[maxBees];
        for (var i = 0; i < maxBees; i++)
        {
            var bee = Instantiate(beePrefab, transform).GetComponent<SmallEntity>();
            _bees[i] = bee;
            bee.gameObject.SetActive(false);
        }
        
        _beePainters = new GameObject[maxBeePainters];
        for (var i = 0; i < maxBeePainters; i++)
        {
            var beePainter = Instantiate(beePainterPrefab, transform);
            _beePainters[i] = beePainter;
            beePainter.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (_randomisePainterParent)
        {
            foreach (var beePainter in _beePainters)
            {
                var randomBee = _ownedBees[UnityEngine.Random.Range(0, _ownerBeesCount)];
                beePainter.transform.parent = randomBee.transform;
                beePainter.transform.position = randomBee.transform.position;
            }
        }
    }

    public void ActivateBee()
    {
        var bee = Array.Find(_bees, b => !b.gameObject.activeSelf);
        if (bee == null) return;
        bee.gameObject.SetActive(true);
        _ownedBees[_ownerBeesCount] = bee;
        
        if (_ownerBeesCount < maxBeePainters)
        {
            var beePainter = _beePainters[_ownerBeesCount];
            beePainter.transform.parent = bee.transform;
            beePainter.transform.localPosition = Vector3.zero;
            beePainter.SetActive(true);
        }
        else if (_ownerBeesCount == maxBeePainters)
            _randomisePainterParent = true;
        
        _ownerBeesCount++;
    }
}

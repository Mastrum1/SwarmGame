using System;
using Unity.VisualScripting;
using UnityEngine;

public class BeesManager : MonoBehaviour
{
    [SerializeField] private int maxBees;
    [SerializeField] private GameObject beePrefab;
    [SerializeField] private int maxBeePainters;
    [SerializeField] private int maxDroneBees = 10;
    [SerializeField] private GameObject beePainterPrefab;
    
    private SmallEntity[] _bees;
    private SmallEntity[] _ownedBees;
    private GameObject[] _beePainters;
    private int _ownerBeesCount;
    private bool _randomisePainterParent = false;
    private int _droneBeesCount = 0;
    
    bool _useAutoPainters = false;

    private void Start()
    {
        _ownerBeesCount = 0;
        _bees = new SmallEntity[maxBees];
        _ownedBees = new SmallEntity[maxBees];
        if (_useAutoPainters) _beePainters = new GameObject[maxBeePainters];
        for (var i = 0; i < maxBees; i++)
        {
            var bee = Instantiate(beePrefab, transform).GetComponent<SmallEntity>();
            _bees[i] = bee;
            bee.gameObject.SetActive(false);
        }

        if (!_useAutoPainters) return;
        for (int i = 0; i < maxBeePainters; i++)
        {
            var beePainter = Instantiate(beePainterPrefab, transform);
            beePainter.SetActive(false);
            _beePainters[i] = beePainter;
        }
    }

    private void Update()
        {
            if (_randomisePainterParent && _useAutoPainters)
            {
                foreach (var beePainter in _beePainters)
                {
                    var randomBee = _ownedBees[UnityEngine.Random.Range(0, _ownerBeesCount)];
                    beePainter.transform.SetParent(randomBee.transform);
                }
            }
        }

        public void ActivateBee()
        {
            _ownerBeesCount++;
            var bee = Array.Find(_bees, b => !b.gameObject.activeSelf);
            if (bee == null) return;
            bee.gameObject.SetActive(true);
            _ownedBees[_ownerBeesCount] = bee;
            if (!_randomisePainterParent && _useAutoPainters)
            {
                var beePainter = Array.Find(_beePainters, b => !b.gameObject.activeSelf);
                if (beePainter == null)
                {
                    _randomisePainterParent = true;
                }
                else
                {
                    beePainter.SetActive(true);
                    beePainter.transform.SetParent(bee.transform);
                }
            }
        }
    
        public void DeactivateBee(SmallEntity bee)
        {
            bee.gameObject.SetActive(false);
            _ownerBeesCount--;
        }
    
        public void ChangeBeesFollowingTarget(Transform target, int amount)
        {
            if (amount > _ownerBeesCount) return;
            if (_droneBeesCount >= maxDroneBees) return;
        
            for (var i = 0; i < amount; i++)
            {
                var bee = Array.Find(_bees, b => !b.IsDrone);
                bee.ChangeFollowingTarget(target);
                bee.IsDrone = true;
            }

            _ownerBeesCount -= amount;
            _droneBeesCount++;
        }

        public int GetBeesCount()
        {
            return _ownerBeesCount;
        }
    
        public int GetDroneBeesCount()
        {
            return _droneBeesCount;
        }
    
        public int GetMaxDroneBees()
        {
            return maxDroneBees;
        }
}

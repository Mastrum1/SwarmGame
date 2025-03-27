using System;
using UnityEngine;

public class TreeEntity : MonoBehaviour
{
    [Header("Tree Settings")]
    [SerializeField] private float growTime = 5f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float maxScale = 1f;
    [SerializeField] private float minScale = 0.1f;

    private bool _isGrowing;
    
    public void InitializeTree(Vector3 position)
    {
        transform.localScale = Vector3.one * minScale;
        transform.position = position;
    }

    private void Update()
    {
        if (!_isGrowing) Grow();
    }

    private void Grow()
    {
        var scale = Mathf.Lerp(minScale, maxScale, Time.time / growTime);
        transform.localScale = Vector3.one * scale;
        
        if (transform.localScale.x >= maxScale)
        {
            transform.localScale = Vector3.one * maxScale;
            _isGrowing = false;
        }
    }
}

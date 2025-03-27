using UnityEngine;
using DG.Tweening;

public class DoTweenOnStart : MonoBehaviour
{
    [SerializeField] private Ease _ease;
    [SerializeField] private float _scaleTime;

    private void Start()
    {
        var scale = transform.localScale;
	transform.localScale = Vector3.zero;
        transform.DOScale(scale, _scaleTime).SetEase(_ease);
    }
}
using UnityEngine;
using DG.Tweening;

public class CameraZoomOutEnd : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [SerializeField] private float _endZoom;
    [SerializeField] private float _unzoomTime;
    [SerializeField] private Ease _ease;

    public void ZoomOut()
    {
        _camera.DOMoveY(_endZoom, _unzoomTime).SetEase(_ease);
    }
}

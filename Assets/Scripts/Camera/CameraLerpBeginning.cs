using Scripts;
using UnityEngine;


public class CameraLerpBeginning : MonoBehaviour
{
    [SerializeField] private CameraController _camera;
    [SerializeField] private float _time;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private AnimationCurve _curve;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        var lerp = _curve.Evaluate(_timer / _time);
        _camera.Speed = Mathf.Lerp(_minSpeed, _maxSpeed, lerp);

        if (_timer > _time)
        {
            Destroy(this);
        }
    }
}
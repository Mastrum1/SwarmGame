using UnityEngine;
using Random = UnityEngine.Random;


public class SmallEntity : MonoBehaviour
{
    [SerializeField] private float _angleOffset;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minRadius;
    [SerializeField] private float _maxRadius;
    [SerializeField] private Vector3 _maxOffset;

    private float _radius;
    private float _currentAngle;
    private float _timerCoolDown;
    private float _speed;
    private Vector3 _offset;
    private Transform _player;
    
    private int _directionInverted;

    private void Start()
    {
        _player = MainGame.Instance.Player.transform;
        _directionInverted = (int)(Random.Range(0, 2) * 0.5 + 0.5);
        _offset = new Vector3(Random.Range(0, _maxOffset.x), Random.Range(0, _maxOffset.y),
            Random.Range(0, _maxOffset.z));
        _speed = Random.Range(_minSpeed, _maxSpeed);
        _radius = Random.Range(_minRadius, _maxRadius);
    }

    public void Update()
    {
        Vector3 direction = new Vector3(Mathf.Cos(_currentAngle), 0, Mathf.Sin(_currentAngle));
        _currentAngle += _angleOffset * _directionInverted * _speed;
        transform.position = _player.transform.position + _offset + Vector3.ProjectOnPlane(direction,_player.up);
        //Random.insideUnitSphere * _radius
    }
}
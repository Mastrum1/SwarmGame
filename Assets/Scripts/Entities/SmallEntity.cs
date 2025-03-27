using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// A gameObject following the Player around a circle
/// </summary>
public class SmallEntity : MonoBehaviour
{
    public Vector3 Direction { get; private set; }
    
    [SerializeField] private float _angleOffset;
    [SerializeField] private float _minSpeed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _minRadius;
    [SerializeField] private float _maxRadius;
    [SerializeField] private float _yOffset;
    [SerializeField] private Vector3 _maxOffset;
    [SerializeField, Range(0,1)] private float _maxLerp;
    [SerializeField, Range(0,1)] private float _minLerp;

    private float _radius;
    private float _currentAngle;
    private float _timerCoolDown;
    private float _speed;
    private Vector3 _offset;
    private float _lerp;
    private int _directionInverted;
    private Transform _player;
    private Vector3 _planetPosition;
    private Vector3 _direction;
    
    private void Start()
    {
        _player = MainGame.Instance.Player.transform;
        _directionInverted = (int)((Random.Range(0, 2) - 0.5) * 2);
        _offset = new Vector3(Random.Range(0, _maxOffset.x), Random.Range(0, _maxOffset.y),
            Random.Range(0, _maxOffset.z));
        _speed = Random.Range(_minSpeed, _maxSpeed);
        _radius = Random.Range(_minRadius, _maxRadius);
        _lerp = Random.Range(_minLerp, _maxLerp);
        _planetPosition = MainGame.Instance.Planet.transform.position;
    }

    public void FixedUpdate()
    {
        Move();
        
        Rotate();
    }

    private void Move()
    {
        var direction = new Vector3(Mathf.Cos(_currentAngle), 0, Mathf.Sin(_currentAngle));
        _currentAngle += _angleOffset * _directionInverted * _speed * Time.deltaTime;
        direction = (_player.right * direction.x + _player.forward * direction.z).normalized;
        var newPosition = _player.position + _offset + direction * _radius + _player.up * _yOffset;
        _direction = newPosition - transform.position;
        var targetPosition = Vector3.Lerp(transform.position, newPosition, _lerp);
        Direction = targetPosition - transform.position;
        transform.position = targetPosition;
    }

    private void Rotate()
    {
        Quaternion rotation = Quaternion.LookRotation(_direction, -_player.transform.up);
        transform.rotation = rotation;


    }
}
using UnityEngine;

/// <summary>
/// Handles the visual rotation and animations of the player character
/// </summary>
public class PlayerVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _playerRigidbody;

    [SerializeField] private Transform _playerVisual;
    [SerializeField] private Transform _helper;

    [Header("Position")]
    [SerializeField, Range(0, 1)] private float _followLerp;

    [Header("Rotation")]
    [SerializeField] private bool _isOrientationInverted;
    [SerializeField] private float _lookAtLerp;

    [Header("Animations")]
    [SerializeField] private string _isWalkingBool;
    [SerializeField] private Animator _animator;

    private Vector3 _lastDirection;

    private void Start()
    {
        _playerVisual.up = -_playerRigidbody.transform.up;
    }

    private void Update()
    {
        _playerVisual.transform.position = Vector3.Lerp(_playerVisual.transform.position, _playerRigidbody.transform.position, _followLerp);
    }

    private void LateUpdate()
    {
        RotateDirection();
    }

    private void RotateDirection()
    {
        Vector3 moveDirection = _playerRigidbody.linearVelocity;
        moveDirection.y = 0;
        var isWalking = moveDirection.sqrMagnitude >= 0.1f;
        if (isWalking) _lastDirection = moveDirection;
        _animator.SetBool(_isWalkingBool, isWalking);
        int orientation = _isOrientationInverted ? -1 : 1;

        Vector3 point = _lastDirection * orientation - _playerRigidbody.position;
        Vector3 direction = _playerRigidbody.position - point;
        if (direction.magnitude < 0.1f) return;
        _playerVisual.up = -_playerRigidbody.transform.up;
        Quaternion toRotation = Quaternion.LookRotation(direction, _playerVisual.up);
        _playerVisual.rotation = Quaternion.Lerp(_playerVisual.rotation, toRotation, _lookAtLerp * Time.deltaTime);
        
    }
}
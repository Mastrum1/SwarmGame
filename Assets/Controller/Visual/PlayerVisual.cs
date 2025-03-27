using UnityEngine;

/// <summary>
/// Handles the visual rotation and animations of the player character
/// </summary>
public class PlayerVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody _playerRigidbody;

    [SerializeField] private Transform _playerVisual;

    [Header("Position")]
    [SerializeField, Range(0,1)] private float _followLerp;
    
    [Header("Rotation")]
    [SerializeField] private bool _isOrientationInverted;
    [SerializeField] private float _lookAtLerp;
    
    [Header("Animations")]
    [SerializeField] private string _isWalkingBool;
    [SerializeField] private Animator _animator;

    private Vector3 _lastDirection;

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
        // Get the player's horizontal movement direction.
        Vector3 moveDirection = _playerRigidbody.linearVelocity;
        moveDirection.y = 0f;

        bool isWalking = moveDirection.sqrMagnitude >= 0.1f;
        if (isWalking)
        {
            // Normalize for consistency.
            _lastDirection = moveDirection.normalized;
        }
        _animator.SetBool(_isWalkingBool, isWalking);

        // If the player is not moving significantly, skip rotation.
        if (!isWalking)
            return;

        // Apply orientation inversion if needed.
        int orientation = _isOrientationInverted ? -1 : 1;
        Vector3 desiredDirection = _lastDirection * orientation;

        // Ensure the desired direction is valid.
        if (desiredDirection == Vector3.zero)
            return;

        // Compute the target rotation.
        Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, _playerRigidbody.transform.up);

        // Smoothly interpolate to the target rotation.
        _playerVisual.rotation = Quaternion.Lerp(_playerVisual.rotation, targetRotation, _lookAtLerp * Time.deltaTime);
    }
}
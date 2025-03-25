using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody rb;
    
    [Header("Planet")]
    [SerializeField] private Transform planetPosition;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float gravityForce = 1f;
    
    private PlayerInputs _playerInputs;
    private InputAction _moveAction;
    private float _planetRadius;
    
    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _moveAction = _playerInputs.Player.Move;
        _planetRadius = planetPosition.localScale.x / 2;
    }
    
    private void OnEnable()
    {
        _playerInputs.Enable();
    }
    
    private void OnDisable()
    {
        _playerInputs.Disable();
    }
    
    private void FixedUpdate()
    {
        Move();
        ApplyGravity();
        RotateToPlanet();
    }

    private void Move()
    {
        Vector2 movement = _moveAction.ReadValue<Vector2>();
        Vector3 moveDirection;
        moveDirection = transform.forward * (movement.x * speed);
        moveDirection += transform.right * (movement.y * speed);
        rb.linearVelocity = moveDirection * speed;
    }

    private void ApplyGravity()
    {
        Vector3 direction = (planetPosition.position - transform.position).normalized;
        rb.AddForce(direction * (gravity * gravityForce));
    }
    
    private void RotateToPlanet()
    {
        Vector3 direction = (planetPosition.position - transform.position).normalized;
        Quaternion rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
        transform.rotation = rotation;
    }
}

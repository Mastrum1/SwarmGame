using System;
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
    [SerializeField] private float gravityStrength = 1f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform groundCollider;
    
    private PlayerInputs _playerInputs;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    
    private RayData _groundData;
    private Vector3 _gravityDirection;

    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _moveAction = _playerInputs.Player.Move;
        _lookAction = _playerInputs.Player.Look;
        _jumpAction = _playerInputs.Player.Jump;
        _groundData = new RayData();
    }

    private void Start()
    {
        _gravityDirection = (planetPosition.position - transform.position).normalized;
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
        ApplyGravity();
        CheckGround();
        RotateToPlanet();
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector2 movement = _moveAction.ReadValue<Vector2>();
        var moveDirection = transform.forward * (movement.x * speed);
        moveDirection += transform.right * (movement.y * speed);
        rb.linearVelocity = moveDirection * speed;
        if (_jumpAction.WasPressedThisFrame()) Jump();
    }
    
    private void Rotate()
    {
        Vector2 mouseDelta = _lookAction.ReadValue<Vector2>();
        var rotation = new Vector3(0, mouseDelta.x, 0);
        transform.Rotate(rotation);
    }
    
    private void Jump()
    {
        if (_groundData.grounded)
        {
            rb.AddForce(transform.up * 10, ForceMode.Impulse);
        }
    }

    private void ApplyGravity()
    {
        var distance = Vector3.Distance(transform.position, planetPosition.position);
        
        if (!_groundData.grounded)
            gravityStrength += gravityStrength * 11 * Time.deltaTime;
        else
            gravityStrength = gravity;
        
        Vector3 direction = (planetPosition.position - transform.position).normalized;
        rb.AddForce(direction * (gravity * gravityStrength * distance));
    }

    private void RotateToPlanet()
    {
        Vector3 direction = (planetPosition.position - transform.position).normalized;
        Quaternion rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
        transform.rotation = rotation;
    }
    
    private void CheckGround()
    {
        if(Physics.CheckSphere(groundCollider.position, 0, groundLayerMask))
        {
            Physics.Raycast(groundCollider.position, -transform.up, out RaycastHit hit, 5f);
            _groundData.grounded = true;
            _groundData.normal = hit.normal;
            return;
        }

        _groundData.grounded = false;
        _groundData.normal = -_gravityDirection;
    }
}

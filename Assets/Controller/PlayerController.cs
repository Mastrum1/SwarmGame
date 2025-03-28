using System;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float Speed
    {
        get => speed;
        set => speed = value;
    }
    
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private AnimationCurve mouseSensitivityCurve;
    
    [Header("Planet")]
    [SerializeField] private Transform planetPosition;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float gravityStrength = 1f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform groundCollider;
    
    [Header("Bees")]
    [SerializeField] private BeesManager beesManager;
    [SerializeField] private Drone dronePrefab;
    
    private PlayerInputs _playerInputs;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _sendAction;
    
    private RayData _groundData;
    private Vector3 _gravityDirection;

    //private int _jumpState = 0;

    private void Awake()
    {
        _playerInputs = new PlayerInputs();
        _moveAction = _playerInputs.Player.Move;
        _lookAction = _playerInputs.Player.Look;
        _jumpAction = _playerInputs.Player.Jump;
        _sendAction = _playerInputs.Player.Plant;
        _groundData = new RayData();
    }

    private void Start()
    {
        _gravityDirection = (planetPosition.position - transform.position).normalized;
        rb.maxLinearVelocity = speed * 2.3f;
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
        CheckForEscape();
        
    }

    private void CheckForEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscape();
        }
    }

    private static void ToggleEscape()
    {
        Cursor.visible = !Cursor.visible;
        if (Cursor.visible == false) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (_sendAction.WasPressedThisFrame()) SendBees();
    }

    private void Move()
    {
        Vector2 movement = _moveAction.ReadValue<Vector2>();
        var moveDirection = transform.forward * (movement.x);
        moveDirection += transform.right * (movement.y);
        rb.linearVelocity += moveDirection.normalized * speed;
        
        if (_jumpAction.WasPressedThisFrame()) Jump();
    }
    
    private void Rotate()
    {
        Vector2 mouseDelta = _lookAction.ReadValue<Vector2>();
        
        var mouseSensitivity = mouseSensitivityCurve.Evaluate(mouseDelta.magnitude);
        var rotation = new Vector3(0, mouseDelta.x * mouseSensitivity, 0);

        if (Gamepad.current == null)
        {
            transform.Rotate(-rotation);
            return;
        }
        rotation = new Vector3(0, mouseDelta.x * 3, 0);
        transform.Rotate(-rotation);
    }
    
    private void Jump()
    {
        if (_groundData.grounded)
        {
            rb.AddForce(transform.up * 1000, ForceMode.Impulse);
        }
    }

    private void SendBees()
    {
        if (beesManager.GetBeesCount() < 5) return;
        if (beesManager.GetDroneBeesCount() >= beesManager.GetMaxDroneBees()) return;
        var drone = Instantiate(dronePrefab, transform.position, Quaternion.identity);
        beesManager.ChangeBeesFollowingTarget(drone.transform, 5);
    }
    
    private void ApplyGravity()
    {
        var distance = Vector3.Distance(transform.position, planetPosition.position);
        
        if (!_groundData.grounded)
            gravityStrength += gravityStrength * Time.deltaTime;
        else
            gravityStrength = gravity;
        
        Vector3 direction = (planetPosition.position - transform.position).normalized;
        rb.AddForce(direction * (gravity * gravityStrength * distance / 10));
    }

    private void RotateToPlanet()
    {
        Vector3 direction = (planetPosition.position - transform.position).normalized;
        Quaternion rotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;
        transform.rotation = rotation;
    }
    
    private void CheckGround()
    {
        if(Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 5f, groundLayerMask) && hit.collider != null)
        {
            _groundData.grounded = true;
            _groundData.normal = hit.normal;
            return;
        }
        else
        {
            _groundData.grounded = false;
            _groundData.normal = -_gravityDirection;
        }

        
    }
}

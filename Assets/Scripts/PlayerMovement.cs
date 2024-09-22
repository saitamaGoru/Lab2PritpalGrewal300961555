using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : ValidatedMonoBehaviour
{
    #region variables
    private PlayerInputs _inputs;

    [SerializeField] private float _speed = 6f;
    [SerializeField, Self] private CharacterController _controller;

    [Header("Movement")]
    [SerializeField] private Vector2 _move;
    [SerializeField] private Vector3 _movement;
    [SerializeField] private Vector3 _velocity;

    [Header("Ground Detection")]
    [SerializeField, Child(Flag.ExcludeSelf)] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMaks;
    [SerializeField] private float _groundRadius = 0.5f;
    [SerializeField] private bool _isGrounded;

    [Header("Jump")]
    [SerializeField] private bool _isJumpPressed;
    [SerializeField] private float _jumpHeight = 3.0f, _initialJumpVelocity, _maxJumpTime = 0.5f;
     
    
    // Constants
    private const float GROUND_GRAVITY = -0.05f;
    private float _gravity = -9.8f;
    
    #endregion
    
    #region Unity Methods
    private void Awake()
    {
        _inputs = new PlayerInputs();
        
       _inputs.Player.Move.performed += ctx => _move = ctx.ReadValue<Vector2>(); 
        _inputs.Player.Move.canceled += cancel => _move = Vector2.zero;
        _inputs.Player.Jump.started += Jump;
        _inputs.Player.Jump.canceled += Jump;

        JumpVariables();
    }
    private void JumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _jumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _jumpHeight) / timeToApex;
    }

    private void OnEnable() => _inputs.Enable();
    private void OnDisable() => _inputs.Disable();

    private void FixedUpdate()
    {
        _movement = (transform.right * _move.x + transform.forward * _move.y) * (_speed * Time.fixedDeltaTime);

        if (!_controller.enabled) { return; }
        
        _controller.Move(_movement);
        OnGravity();
        _controller.Move(_velocity * Time.fixedDeltaTime);
    }

    
    #endregion

    #region Player Methods
    
    private void Jump(InputAction.CallbackContext callbackContext)
    {
        _isJumpPressed = callbackContext.ReadValueAsButton();
        if (_isGrounded && _isJumpPressed)
        {
            _velocity.y = _initialJumpVelocity;
        }

    }
    
    private void OnGravity()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundRadius, _groundMaks);
        
        if (_isGrounded && _velocity.y < 0.0f)
        {
            _velocity.y = GROUND_GRAVITY;
        }
        else
        {
            _velocity.y += _gravity;
        }
    }
    
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _camSensitivity = 100f;
    [SerializeField, Parent(Flag.Editable)] private Transform _playerBody;
    [SerializeField] private Vector2 _lookInput;
    [SerializeField] private float _xRotation, _xLookInput, _yLookInput;
    private PlayerInputs _playerInputs;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _playerInputs = new PlayerInputs();
        _playerInputs.Player.Look.started += OnLookInput;
        _playerInputs.Player.Look.performed += OnLookInput;
        _playerInputs.Player.Look.canceled += OnLookInput;
    }

    private void OnEnable() => _playerInputs.Enable();
    private void OnDisable() => _playerInputs.Disable();
    
    private void OnLookInput(InputAction.CallbackContext ctx)
    {
        _lookInput = ctx.ReadValue<Vector2>();
        _xLookInput = _lookInput.x * _camSensitivity;
        _yLookInput = _lookInput.y * _camSensitivity;
    }

    private void FixedUpdate()
    {
        _xRotation -= _yLookInput  * Time.fixedDeltaTime;
        _xRotation = Mathf.Clamp(_xRotation, -85f, 85f);
        
        transform.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);
        _playerBody.Rotate(Vector3.up * _xLookInput * Time.fixedDeltaTime);
    }
}

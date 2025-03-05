using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerInput _input;
    private Camera _cam;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;

    private Vector2 _curInput;
    public float moveSpeed;
    public float jumpPower;

    private Vector2 _mouseDelta;
    private float _camCurXRot;
    private float _camDistance = 5f;
    private float _lookSens = 0.1f;
    private float _minXLook = -70;
    private float _maxXLook = 70;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
        _cam = Camera.main;

        _moveAction = _input.actions["Move"];
        _jumpAction = _input.actions["Jump"];
        _lookAction = _input.actions["Look"];

        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _jumpAction.started -= OnJump;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _jumpAction.started += OnJump;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        Look();
    }

    void Move()
    {
        Vector3 dir = transform.forward * _curInput.y + transform.right * _curInput.x;
        dir *= moveSpeed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) _curInput = context.ReadValue<Vector2>().normalized;
        else if(context.canceled) _curInput = Vector2.zero;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed) _mouseDelta = context.ReadValue<Vector2>();
        else if (context.canceled) _mouseDelta = Vector2.zero;

        Debug.Log(_mouseDelta);
    }

    void Look()
    {
        _camCurXRot += _mouseDelta.y * _lookSens;
        _camCurXRot = Mathf.Clamp(_camCurXRot, _minXLook, _maxXLook);
        _cam.transform.localEulerAngles = new Vector3(-_camCurXRot, 0, 0);

        Vector3 c = transform.position + new Vector3(0, 1, 0);
        c -= _cam.transform.forward * _camDistance;
        _cam.transform.position = c;

        transform.eulerAngles += new Vector3(0, _mouseDelta.x * _lookSens);
    }
}

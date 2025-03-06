using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerInput _input;
    private Camera _cam;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem dustParticleSystem;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _InvestigateAction;

    private Vector2 _curInput;
    public float moveSpeed;
    public float maxSpeed;
    public float jumpPower;
    //[HideInInspector] public bool isMovable;
    LayerMask groundLayer;

    private Vector2 _mouseDelta;
    private float _camCurXRot;
    private float _camDistance = 6f;
    private float _lookSens = 0.1f;
    private float _minXLook = -60;
    private float _maxXLook = 60;

    [HideInInspector] public Action OnMouseClicked;
    [HideInInspector] public Action OnMouseCanceled;
    [HideInInspector] public Action<string> OnItemFound;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
        _cam = Camera.main;

        groundLayer = LayerMask.GetMask("Ground");

        _moveAction = _input.actions["Move"];
        _jumpAction = _input.actions["Jump"];
        _lookAction = _input.actions["Look"];
        _InvestigateAction = _input.actions["Investigate"];

        _moveAction.performed -= OnMove;
        _moveAction.canceled -= OnMove;
        _jumpAction.started -= OnJump;
        _lookAction.performed -= OnLook;
        _lookAction.canceled -= OnLook;
        _InvestigateAction.performed -= OnInvestigate;
        _InvestigateAction.canceled -= OnInvestigate;

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _jumpAction.started += OnJump;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        _InvestigateAction.performed += OnInvestigate;
        _InvestigateAction.canceled += OnInvestigate;

        dustParticleSystem.Stop();
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
        Vector3 horVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        float horSpeed = horVelocity.magnitude;
        if (horSpeed > maxSpeed)
        {
            //cos 계산 후 각도에 따라 다른 방향일 때만 힘 적용
            float cos = Vector3.Dot(dir.normalized, horVelocity.normalized);
            float weight = (1 - cos)/ 4;

            _rb.AddForce(dir * weight, ForceMode.Impulse);
            Debug.Log($"수평 속도: {horSpeed}, 가중치: {weight}");
        }
        else
        {
            _rb.AddForce(dir, ForceMode.Impulse);
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _animator.SetBool("isMoving", true);
            if(isGrounded())dustParticleSystem.Play();
            _curInput = context.ReadValue<Vector2>().normalized;
        }
        else if (context.canceled)
        {
            _animator.SetBool("isMoving", false);
            dustParticleSystem.Stop();
            _curInput = Vector2.zero;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (!isGrounded()) return;

        _animator.SetBool("isJumping", true);
        dustParticleSystem.Stop();
        _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    bool isGrounded()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.5f, groundLayer)) return true;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _animator.SetBool("isJumping", false);
            dustParticleSystem.Play();
        }
    }

    void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed) _mouseDelta = context.ReadValue<Vector2>();
        else if (context.canceled) _mouseDelta = Vector2.zero;
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

    void OnInvestigate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnMouseClicked?.Invoke();
            Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), _cam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 2f, groundLayer))
            {
                Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red, .1f);
                string info = hit.collider.gameObject.name;
                OnItemFound?.Invoke(info);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 2f, Color.blue, .1f);
            }
        }
        else if (context.canceled)
        {
            OnMouseCanceled?.Invoke();
        }
    }
}

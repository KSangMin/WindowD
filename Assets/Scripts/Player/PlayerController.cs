using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region �ʵ�

    private PlayerCondition _condition;
    private Collider _collider;
    private Rigidbody _rb;
    private PlayerInput _input;
    private Camera _cam;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _dustParticleSystem;

    //�Է�
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _InvestigateAction;

    //�̵�
    private Vector2 _curInput;
    public float moveSpeed;
    public float maxSpeed;
    [HideInInspector] public Action<float> OnSpeedChanged;
    
    //����
    public float jumpPower;
    public float jumpStamina;
    //[HideInInspector] public bool isMovable;
    LayerMask groundLayer;
    bool isJumping;

    //ȸ��
    private Vector2 _mouseDelta;
    private float _camCurXRot;
    private float _camDistance = 6f;
    private float _lookSens = 0.1f;
    private float _minXLook = -60;
    private float _maxXLook = 60;
    private bool _canLook;

    //�� ������
    public PhysicMaterial normalMaterial;
    public PhysicMaterial zeroFrictionMaterial;

    //��Ÿ��
    bool hangedAlready;
    bool isHanging;
    float hangTimer;
    float maxHangTime = 1f;

    #endregion �ʵ�

    private void Awake()
    {
        _condition = GetComponent<PlayerCondition>();
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
        _cam = Camera.main;

        groundLayer = LayerMask.GetMask("Ground");

        ResetActions();

        _dustParticleSystem.Stop();
    }

    //InputAction Event �ʱ�ȭ
    void ResetActions()
    {
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

        _moveAction.performed += OnMove;
        _moveAction.canceled += OnMove;
        _jumpAction.started += OnJump;
        _lookAction.performed += OnLook;
        _lookAction.canceled += OnLook;
        _InvestigateAction.performed += OnInvestigate;
    }

    private void Start()
    {
        SetCanLook(true);
    }

    private void FixedUpdate()
    {
        if (!_canLook) return;

        if (isHanging)//��Ÿ�� ����
        {
            hangTimer += Time.deltaTime;
            if (hangTimer >= maxHangTime) EndWallHanging();
            MoveVertical();
        }
        else
        {
            Move();
        }

        //�ӵ��� UI
        OnSpeedChanged?.Invoke(_rb.velocity.magnitude);
    }

    private void LateUpdate()
    {
        if(_canLook) Look();
    }

    #region �̵�

    //�����̵�
    void Move()
    {
        Vector3 dir = transform.forward * _curInput.y + transform.right * _curInput.x;
        dir *= moveSpeed;
        Vector3 horVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        float horSpeed = horVelocity.magnitude;
        if (horSpeed > maxSpeed)
        {
            //cos ��� �� ������ ���� �ٸ� ������ ���� �� ����
            float cos = Vector3.Dot(dir.normalized, horVelocity.normalized);
            float weight = (1 - cos) / 4;

            _rb.AddForce(dir * weight, ForceMode.Impulse);
            //Debug.Log($"���� �ӵ�: {horSpeed}, ����ġ: {weight}");
        }
        else
        {
            _rb.AddForce(dir, ForceMode.Impulse);
        }
    }

    //�����̵�
    void MoveVertical()
    {
        Vector3 dir = transform.up * _curInput.y + transform.right * _curInput.x;
        dir *= moveSpeed;
        dir.z = _rb.velocity.z;
        _rb.velocity = dir;
    }

    //Ű���� ����Ű �Է�
    void OnMove(InputAction.CallbackContext context)
    {
        if (_canLook && context.performed)
        {
            _animator.SetBool("isMoving", true);
            if(isGrounded())_dustParticleSystem.Play();
            _curInput = context.ReadValue<Vector2>().normalized;
        }
        else if (context.canceled)
        {
            _animator.SetBool("isMoving", false);
            _dustParticleSystem.Stop();
            _curInput = Vector2.zero;
        }
    }

    public void ExecuteRun(float time, float runSpeed)
    {
        StartCoroutine(Run(time, runSpeed));
    }

    //�޸���
    IEnumerator Run(float time, float runSpeed)
    {
        float originalSpeed = moveSpeed;
        moveSpeed = runSpeed;
        yield return new WaitForSeconds(time);
        moveSpeed = originalSpeed;
    }

    #endregion �̵�

    #region ����, ��Ÿ��

    //Ű���� �����̽��� �Է�
    void OnJump(InputAction.CallbackContext context)
    {
        if (!isGrounded() || isHanging) return;
        if (!_condition.UseStamina(jumpStamina)) return;

        _animator.SetBool("isJumping", true);
        _dustParticleSystem.Stop();
        Jump(Vector3.up);
    }

    void Jump(Vector3 jumpDir)
    {
        isJumping = true;
        _rb.AddForce(jumpDir * jumpPower, ForceMode.Impulse);
    }

    //�ٴ� ���� ����
    bool isGrounded()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 0.2f, Color.red, 5f);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.2f, groundLayer)) return true;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ��������
        if (isGrounded())
        {
            isJumping = false;
            hangedAlready = false;
            _animator.SetBool("isJumping", false);
        }

        //�����̴� �÷��� ����
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            if (collision.contacts[0].normal.y >= 0.5f)//�����̸� 
            {
                transform.SetParent(collision.transform);
            }

        }

        //�� ����
        CheckWall(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        CheckWall(collision);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }

        _collider.material = normalMaterial;
        if(hangedAlready) EndWallHanging();
    }

    //�� ����
    void CheckWall(Collision collision)
    {
        bool isWall = collision.contacts[0].normal.y < 0.5f;

        _collider.material = isWall ? zeroFrictionMaterial : normalMaterial;

        if(isWall && isJumping && !hangedAlready)
        {
            isJumping = false;
            BeginWallHanging();
        }
    }

    //��Ÿ�� ���� -> Fixedupdate���� �ð� üũ
    void BeginWallHanging()
    {
        hangedAlready = true;
        isHanging = true;
        hangTimer = 0;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;

        Debug.Log("��Ÿ�� ����");
    }

    //��Ÿ�� ��
    void EndWallHanging()
    {
        isHanging = false;
        _rb.useGravity = true;
        hangTimer = maxHangTime;

        Debug.Log("��Ÿ�� ��");
    }

    #endregion ����, ��Ÿ��

    #region ���콺

    //���콺 ������ �Է�
    void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed) _mouseDelta = context.ReadValue<Vector2>();
        else if (context.canceled) _mouseDelta = Vector2.zero;
    }

    //ī�޶� �� �÷��̾� ȸ��
    void Look()
    {
        _camCurXRot += _mouseDelta.y * _lookSens;
        _camCurXRot = Mathf.Clamp(_camCurXRot, _minXLook, _maxXLook);
        _cam.transform.localEulerAngles = new Vector3(-_camCurXRot, 0, 0);

        Vector3 c = transform.position + new Vector3(0, 2, 0);
        c -= _cam.transform.forward * _camDistance;
        _cam.transform.position = c;

        transform.eulerAngles += new Vector3(0, _mouseDelta.x * _lookSens);
    }

    public void SetCanLook(bool flag)
    {
        _canLook = flag;
        Cursor.lockState = _canLook ? CursorLockMode.Locked : CursorLockMode.None;
    }

    //���콺 ��Ŭ�� �Է�
    void OnInvestigate(InputAction.CallbackContext context)
    {
        if (!_canLook) return;

        if (context.performed)
        {
            float distanceToHead = Vector3.Distance(transform.position, _cam.transform.position);
            Ray ray = new Ray(_cam.transform.position + _cam.transform.forward * distanceToHead, _cam.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, .1f);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f) && hit.collider.gameObject.TryGetComponent<FloatingItem>(out FloatingItem item))
            {
                string info = item.itemData.displayName + "\n" + item.itemData.description;
                var ui = UIManager.Instance.ShowPopupUI<UI_Info>();
                ui.SetInfoText(info);
                ui.destroyAction += () => SetCanLook(true);
                SetCanLook(false);
            }
        }
    }

    #endregion ���콺
}

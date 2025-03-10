using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region �ʵ�

    private Player _player;
    private PlayerCondition _condition;
    private InputHandler _inputHandler;
    private CameraController _cameraController;
    private Collider _collider;
    private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _dustParticleSystem;

    //�̵�
    private Vector2 _curInput;
    public float originalSpeed;
    private float _moveSpeed;
    public float originalMaxSpeed;
    private float _maxSpeed;
    [HideInInspector] public Action<float> OnSpeedChanged;

    //����
    public float jumpPower;
    public float jumpStamina;
    //[HideInInspector] public bool isMovable;
    LayerMask groundLayer;
    bool isJumping;
    float jumpCount;
    public float maxJumpCount = 1;

    //�� ������
    public PhysicMaterial normalMaterial;
    public PhysicMaterial zeroFrictionMaterial;

    //��Ÿ��
    bool hangedAlready;
    bool isHanging;
    float hangTimer;
    float maxHangTime = 1f;

    //������
    float runTime;
    float doubleJumpTime;
    float invincibleTime;

    #endregion �ʵ�

    #region �̺�Ʈ �Լ�

    private void Awake()
    {
        _player = GetComponent<Player>();
        _condition = GetComponent<PlayerCondition>();
        _inputHandler = GetComponent<InputHandler>();
        _cameraController = GetComponent<CameraController>();

        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        _moveSpeed = originalSpeed;
        _maxSpeed = originalMaxSpeed;

        groundLayer = LayerMask.GetMask("Ground");

        ResetActions();

        _dustParticleSystem.Stop();
    }

    //InputAction Event �ʱ�ȭ
    void ResetActions()
    {
        _inputHandler.moveAction.performed -= OnMove;
        _inputHandler.moveAction.canceled -= OnMove;
        _inputHandler.jumpAction.started -= OnJump;

        _inputHandler.moveAction.performed += OnMove;
        _inputHandler.moveAction.canceled += OnMove;
        _inputHandler.jumpAction.started += OnJump;
    }

    private void FixedUpdate()
    {
        CheckItemTime();

        if (!_player.canLook) return;

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

    void CheckItemTime()
    {
        if (runTime > 0)
        {
            runTime -= Time.deltaTime;
            if (runTime <= 0)
            {
                _moveSpeed = originalSpeed;
                _maxSpeed = originalMaxSpeed;
                Debug.Log("�޸��� ��");
            }
        }

        if (doubleJumpTime > 0)
        {
            doubleJumpTime -= Time.deltaTime;
            if (doubleJumpTime <= 0)
            {
                maxJumpCount = 1;
                Debug.Log("�������� ��");
            }
        }

        if (invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0)
            {
                _player.isInvincible = false;
                Debug.Log("���� ��");
            }
        }
    }

    #endregion �̺�Ʈ �Լ�

    #region �̵�

    //�����̵�
    void Move()
    {
        Vector3 dir = transform.forward * _curInput.y + transform.right * _curInput.x;
        dir *= _moveSpeed;
        Vector3 horVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        float horSpeed = horVelocity.magnitude;
        if (horSpeed > _maxSpeed)
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
        dir *= _moveSpeed;
        dir.z = _rb.velocity.z;
        _rb.velocity = dir;
    }

    //Ű���� ����Ű �Է�
    void OnMove(InputAction.CallbackContext context)
    {
        if (_player.canLook && context.performed)
        {
            _animator.SetBool("isMoving", true);
            if (isGrounded()) _dustParticleSystem.Play();
            _curInput = context.ReadValue<Vector2>().normalized;
        }
        else if (context.canceled)
        {
            _animator.SetBool("isMoving", false);
            _dustParticleSystem.Stop();
            _curInput = Vector2.zero;
        }
    }

    public void BecomeRunnable(float time, float runSpeed)
    {
        runTime = time;
        _maxSpeed = _moveSpeed + runSpeed;
        _moveSpeed += runSpeed;
    }

    #endregion �̵�

    #region ����, ��Ÿ��

    //Ű���� �����̽��� �Է�
    void OnJump(InputAction.CallbackContext context)
    {
        if (isHanging || jumpCount >= maxJumpCount) return;
        if (!_condition.UseStamina(jumpStamina)) return;

        Jump(Vector3.up);
    }

    void Jump(Vector3 jumpDir)
    {
        jumpCount++;
        isJumping = true;
        _animator.SetBool("isJumping", true);
        _dustParticleSystem.Stop();
        _rb.AddForce(jumpDir * jumpPower, ForceMode.Impulse);
    }

    public void BecomeDoubleJumpable(float time, int count = 2)
    {
        doubleJumpTime = time;
        maxJumpCount = count;
    }

    //�ٴ� ���� ����
    bool isGrounded()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down);
        //Debug.DrawRay(ray.origin, ray.direction * 0.2f, Color.red, 5f);
        return Physics.Raycast(ray, 0.2f, groundLayer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ��������
        if (isGrounded())
        {
            jumpCount = 0;
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

    public void BecomeInvincible(float time)
    {
        invincibleTime = time;
        _player.isInvincible = true;
    }
}

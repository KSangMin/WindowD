using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region 필드

    private Player _player;
    private PlayerCondition _condition;
    private InputHandler _inputHandler;
    private CameraController _cameraController;
    private Collider _collider;
    private Rigidbody _rb;
    [SerializeField] private Animator _animator;
    [SerializeField] private ParticleSystem _dustParticleSystem;

    //이동
    private Vector2 _curInput;
    public float originalSpeed;
    private float _moveSpeed;
    public float originalMaxSpeed;
    private float _maxSpeed;
    [HideInInspector] public Action<float> OnSpeedChanged;

    //점프
    public float jumpPower;
    public float jumpStamina;
    //[HideInInspector] public bool isMovable;
    LayerMask groundLayer;
    bool isJumping;
    float jumpCount;
    public float maxJumpCount = 1;

    //벽 마찰력
    public PhysicMaterial normalMaterial;
    public PhysicMaterial zeroFrictionMaterial;

    //벽타기
    bool hangedAlready;
    bool isHanging;
    float hangTimer;
    float maxHangTime = 1f;

    //아이템
    float runTime;
    float doubleJumpTime;
    float invincibleTime;

    #endregion 필드

    #region 이벤트 함수

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

    //InputAction Event 초기화
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

        if (isHanging)//벽타기 진행
        {
            hangTimer += Time.deltaTime;
            if (hangTimer >= maxHangTime) EndWallHanging();
            MoveVertical();
        }
        else
        {
            Move();
        }

        //속도계 UI
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
                Debug.Log("달리기 끝");
            }
        }

        if (doubleJumpTime > 0)
        {
            doubleJumpTime -= Time.deltaTime;
            if (doubleJumpTime <= 0)
            {
                maxJumpCount = 1;
                Debug.Log("더블점프 끝");
            }
        }

        if (invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
            if (invincibleTime <= 0)
            {
                _player.isInvincible = false;
                Debug.Log("무적 끝");
            }
        }
    }

    #endregion 이벤트 함수

    #region 이동

    //수평이동
    void Move()
    {
        Vector3 dir = transform.forward * _curInput.y + transform.right * _curInput.x;
        dir *= _moveSpeed;
        Vector3 horVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
        float horSpeed = horVelocity.magnitude;
        if (horSpeed > _maxSpeed)
        {
            //cos 계산 후 각도에 따라 다른 방향일 때만 힘 적용
            float cos = Vector3.Dot(dir.normalized, horVelocity.normalized);
            float weight = (1 - cos) / 4;

            _rb.AddForce(dir * weight, ForceMode.Impulse);
            //Debug.Log($"수평 속도: {horSpeed}, 가중치: {weight}");
        }
        else
        {
            _rb.AddForce(dir, ForceMode.Impulse);
        }
    }

    //수직이동
    void MoveVertical()
    {
        Vector3 dir = transform.up * _curInput.y + transform.right * _curInput.x;
        dir *= _moveSpeed;
        dir.z = _rb.velocity.z;
        _rb.velocity = dir;
    }

    //키보드 방향키 입력
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

    #endregion 이동

    #region 점프, 벽타기

    //키보드 스페이스바 입력
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

    //바닥 착지 판정
    bool isGrounded()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.1f, 0), Vector3.down);
        //Debug.DrawRay(ray.origin, ray.direction * 0.2f, Color.red, 5f);
        return Physics.Raycast(ray, 0.2f, groundLayer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 점프판정
        if (isGrounded())
        {
            jumpCount = 0;
            isJumping = false;
            hangedAlready = false;
            _animator.SetBool("isJumping", false);
        }

        //움직이는 플랫폼 판정
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            if (collision.contacts[0].normal.y >= 0.5f)//윗면이면 
            {
                transform.SetParent(collision.transform);
            }

        }

        //벽 판정
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

    //벽 판정
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

    //벽타기 시작 -> Fixedupdate에서 시간 체크
    void BeginWallHanging()
    {
        hangedAlready = true;
        isHanging = true;
        hangTimer = 0;
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;

        Debug.Log("벽타기 시작");
    }

    //벽타기 끝
    void EndWallHanging()
    {
        isHanging = false;
        _rb.useGravity = true;
        hangTimer = maxHangTime;

        Debug.Log("벽타기 끝");
    }

    #endregion 점프, 벽타기

    public void BecomeInvincible(float time)
    {
        invincibleTime = time;
        _player.isInvincible = true;
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private Player _player;
    private InputHandler _inputHandler;
    private Camera _cam;

    //회전
    private Vector2 _mouseDelta;
    private float _camCurXRot;
    private float _camDistance = 6f;
    private float _lookSens = 0.1f;
    private float _minXLook = -60;
    private float _maxXLook = 60;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _inputHandler = GetComponent<InputHandler>();

        _cam = Camera.main;
    }

    private void Start()
    {
        _player.SetCanLook(true);

        ResetActions();
    }

    //InputAction Event 초기화
    void ResetActions()
    {
        _inputHandler.lookAction.performed -= OnLook;
        _inputHandler.lookAction.canceled -= OnLook;
        _inputHandler.investigateAction.performed -= OnInvestigate;

        _inputHandler.lookAction.performed += OnLook;
        _inputHandler.lookAction.canceled += OnLook;
        _inputHandler.investigateAction.performed += OnInvestigate;
    }

    private void LateUpdate()
    {
        if (_player.canLook) Look();
    }

    //마우스 움직임 입력
    void OnLook(InputAction.CallbackContext context)
    {
        if (context.performed) _mouseDelta = context.ReadValue<Vector2>();
        else if (context.canceled) _mouseDelta = Vector2.zero;
    }

    //카메라 및 플레이어 회전
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

    //마우스 좌클릭 입력
    void OnInvestigate(InputAction.CallbackContext context)
    {
        if (!_player.canLook) return;

        if (context.performed)
        {
            float distanceToHead = Vector3.Distance(transform.position, _cam.transform.position);
            Ray ray = new Ray(_cam.transform.position + _cam.transform.forward * distanceToHead, _cam.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, .1f);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f) && hit.collider.gameObject.TryGetComponent<FloatingItem>(out FloatingItem item))
            {
                string info = item.itemData.displayName + "\n" + item.itemData.description;
                var ui = UIManager.Instance.ShowUI<UI_Info>();
                ui.SetInfoText(info);
                ui.closeButton.onClick.AddListener(() => _player.SetCanLook(true));
                _player.SetCanLook(false);
            }
        }
    }
}

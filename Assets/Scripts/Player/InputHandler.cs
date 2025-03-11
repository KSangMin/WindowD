using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInput _input;

    //ют╥б
    [HideInInspector] public InputAction moveAction;
    [HideInInspector] public InputAction jumpAction;
    [HideInInspector] public InputAction lookAction;
    [HideInInspector] public InputAction investigateAction;
    [HideInInspector] public InputAction interactAction;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();

        moveAction = _input.actions["Move"];
        jumpAction = _input.actions["Jump"];
        lookAction = _input.actions["Look"];
        investigateAction = _input.actions["Investigate"];
        interactAction = _input.actions["Interact"];
    }
}

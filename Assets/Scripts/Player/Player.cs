using System;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public PlayerCondition condition;
    [HideInInspector] public InputHandler inputHandler;

    public bool isInvincible;
    public bool canLook;

    public Action<ItemData> OnItemChanged;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        inputHandler = GetComponent<InputHandler>();

        GameManager.Instance.player = this;
    }

    public void SetCanLook(bool flag)
    {
        canLook = flag;
        Cursor.lockState = canLook ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetItem(ItemData item)
    {
        OnItemChanged?.Invoke(item);
    }
}

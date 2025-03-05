using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        GameManager.Instance.player = this;
    }
}

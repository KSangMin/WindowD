using UnityEngine;

public class DoubleJump : FloatingItem
{
    public float time = 3;
    public int jumpCount = 2;

    private void OnTriggerEnter(Collider other)
    {
        ApplyItem();
    }

    public override void ApplyItem()
    {
        GameManager.Instance.player.controller.BecomeDoubleJumpable(time, jumpCount);
        Destroy(gameObject);
    }
}

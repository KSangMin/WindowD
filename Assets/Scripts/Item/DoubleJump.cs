using UnityEngine;

public class DoubleJump : FloatingItem
{
    public int jumpCount = 2;

    private void OnTriggerEnter(Collider other)
    {
        ApplyItem();
    }

    public override void ApplyItem()
    {
        base.ApplyItem();

        GameManager.Instance.player.controller.BecomeDoubleJumpable(jumpCount);
        //Destroy(gameObject);
    }
}

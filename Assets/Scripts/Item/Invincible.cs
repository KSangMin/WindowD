using UnityEngine;

public class Invincible : FloatingItem
{
    private void OnTriggerEnter(Collider other)
    {
        ApplyItem();
    }

    public override void ApplyItem()
    {
        base.ApplyItem();

        GameManager.Instance.player.controller.BecomeInvincible();
        //Destroy(gameObject);
    }
}

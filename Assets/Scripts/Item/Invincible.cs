using UnityEngine;

public class Invincible : FloatingItem
{
    public float time = 3;

    private void OnTriggerEnter(Collider other)
    {
        ApplyItem();
    }

    public override void ApplyItem()
    {
        GameManager.Instance.player.controller.BecomeInvincible(time);
        Destroy(gameObject);
    }
}

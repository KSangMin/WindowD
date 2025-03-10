using UnityEngine;

public class Booster : FloatingItem
{
    public float boosterSpeed;

    private void OnTriggerEnter(Collider other)
    {
        ApplyItem();
    }

    public override void ApplyItem()
    {
        base.ApplyItem();

        GameManager.Instance.player.controller.BecomeRunnable(boosterSpeed);
        //Destroy(gameObject);
    }
}

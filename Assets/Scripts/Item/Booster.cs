using UnityEngine;

public class Booster : FloatingItem
{
    public float boosterTime;
    public float boosterSpeed;

    private void OnTriggerEnter(Collider other)
    {
        ApplyItem();
    }

    public override void ApplyItem()
    {
        GameManager.Instance.player.controller.ExecuteRun(boosterTime, boosterSpeed);
        Destroy(gameObject);
    }
}

using System.Collections;
using UnityEngine;

public class UI_DamageIndicator : UI
{
    protected override void Awake()
    {
        base.Awake();

        Hide();
    }

    public void Hitted()
    {
        if (GameManager.Instance.player.isInvincible) return;

        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        Show();

        yield return new WaitForSeconds(0.3f);

        Hide();
    }
}

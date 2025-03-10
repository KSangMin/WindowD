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
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        Show();

        yield return new WaitForSeconds(0.3f);

        Hide();
    }
}

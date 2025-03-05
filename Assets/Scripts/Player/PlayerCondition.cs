using System;
using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public Stat health = new Stat(100, 100);

    private void FixedUpdate()
    {
        health.Subtract(Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        health.Subtract(damage);
        if(health.CurValue <= 0)
        {
            Dead();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    void Dead()
    {
        Debug.Log("ав╬З╢ы!");
    }
}

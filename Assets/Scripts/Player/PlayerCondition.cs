using System;
using UnityEngine;

public class PlayerCondition : MonoBehaviour, IDamageable
{
    private Player _player;

    public Stat health = new Stat(100, 100);
    public Stat stamina = new Stat(100, 100);

    private void Start()
    {
        _player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.player.canLook == false) return;

        health.Subtract(Time.deltaTime);
        stamina.Add(Time.deltaTime);
    }

    public bool UseStamina(float amount)
    {
        if (stamina.CurValue < amount) return false;
        
        stamina.Subtract(amount);
        return true;
    }

    public void TakeDamage(float damage)
    {
        if (_player.isInvincible) return;

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

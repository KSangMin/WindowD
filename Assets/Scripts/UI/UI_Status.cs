using UnityEngine;
using UnityEngine.UI;

public class UI_Status : UI_Base
{
    public Image ImgHealthBar;
    public Image ImgStaminaBar;

    private void Start()
    {
        GameManager.Instance.player.condition.health.OnStatChangedWithFloat -= OnHealthChanged;
        GameManager.Instance.player.condition.stamina.OnStatChangedWithFloat -= OnStaminaChanged;

        GameManager.Instance.player.condition.health.OnStatChangedWithFloat += OnHealthChanged;
        GameManager.Instance.player.condition.stamina.OnStatChangedWithFloat += OnStaminaChanged;
    }

    void OnHealthChanged(float ratio)
    {
        ImgHealthBar.fillAmount = ratio;
    }

    void OnStaminaChanged(float ratio)
    {
        ImgStaminaBar.fillAmount = ratio;
    }

    public override void Close()
    {
        GameManager.Instance.player.condition.health.OnStatChangedWithFloat -= OnHealthChanged;
        GameManager.Instance.player.condition.stamina.OnStatChangedWithFloat -= OnStaminaChanged;

        base.Close();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Status : UI_Base
{
    public Image imgHealthBar;
    public Image imgStaminaBar;

    public Image imgSpeedMeter;
    public TextMeshProUGUI speedText;

    private void Start()
    {
        GameManager.Instance.player.condition.health.OnStatChangedWithFloat -= OnHealthChanged;
        GameManager.Instance.player.condition.stamina.OnStatChangedWithFloat -= OnStaminaChanged;
        GameManager.Instance.player.controller.OnSpeedChanged -= OnSpeedChanged;

        GameManager.Instance.player.condition.health.OnStatChangedWithFloat += OnHealthChanged;
        GameManager.Instance.player.condition.stamina.OnStatChangedWithFloat += OnStaminaChanged;
        GameManager.Instance.player.controller.OnSpeedChanged += OnSpeedChanged;
    }

    void OnHealthChanged(float ratio)
    {
        imgHealthBar.fillAmount = ratio;
    }

    void OnStaminaChanged(float ratio)
    {
        imgStaminaBar.fillAmount = ratio;
    }

    void OnSpeedChanged(float curValue)
    {
        imgSpeedMeter.fillAmount = Mathf.Clamp(curValue / GameManager.Instance.player.controller.maxSpeed, 0.145f, 1f);
        speedText.text = ((int)curValue).ToString();
    }

    public override void Destroy()
    {
        GameManager.Instance.player.condition.health.OnStatChangedWithFloat -= OnHealthChanged;
        GameManager.Instance.player.condition.stamina.OnStatChangedWithFloat -= OnStaminaChanged;
        GameManager.Instance.player.controller.OnSpeedChanged += OnSpeedChanged;

        base.Destroy();
    }
}

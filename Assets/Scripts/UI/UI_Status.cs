using UnityEngine;
using UnityEngine.UI;

public class UI_Status : UI_Base
{
    public Image ImgHealthBar;

    void OnHealthChanged(float ratio)
    {
        ImgHealthBar.fillAmount = ratio;
    }
}

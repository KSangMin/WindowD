using TMPro;
using UnityEngine;

public class UI_InteractableIndicator : UI
{
    public TextMeshProUGUI infoText;

    public void SetInfoText(string info)
    {
        infoText.text = info;
    }
}

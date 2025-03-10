using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Info : UI
{
    public Button closeButton;

    public TextMeshProUGUI InfoText;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public void SetInfoText(string info)
    {
        InfoText.text = info;
    }
}

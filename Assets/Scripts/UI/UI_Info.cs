using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Info : UI_Popup
{
    public Button closeButton;

    public TextMeshProUGUI InfoText;

    private void Start()
    {
        closeButton.onClick.AddListener(Destroy);
    }

    public void SetInfoText(string info)
    {
        InfoText.text = info;
    }
}

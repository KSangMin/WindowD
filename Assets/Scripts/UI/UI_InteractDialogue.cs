using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteractDialogue : UI
{
    public Button closeButton;
    public TextMeshProUGUI descriptionText;

    protected override void Awake()
    {
        base.Awake();

        Hide();
        closeButton.onClick.AddListener(Close);
    }

    public void ShowDialogue(string info)
    {
        descriptionText.text = info;
        GameManager.Instance.player.SetCanLook(false);
        Show();
    }

    void Close()
    {
        GameManager.Instance.player.SetCanLook(true);
        Hide();
    }
}

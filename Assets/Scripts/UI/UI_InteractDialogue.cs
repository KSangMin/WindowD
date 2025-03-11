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

        Resume();
        closeButton.onClick.AddListener(Resume);
    }

    public void ShowDialogue(string info)
    {
        descriptionText.text = info;
        GameManager.Instance.player.SetCanLook(false);
        Show();
    }

    void Resume()
    {
        GameManager.Instance.player.SetCanLook(true);
        Hide();
    }
}

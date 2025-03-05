using TMPro;
using UnityEngine;

public class UI_Info : UI_Base
{
    public TextMeshProUGUI InfoText;
    PlayerController controller;

    private void Start()
    {
        controller = GameManager.Instance.player.controller;

        controller.OnItemFound -= OnItemFound;
        controller.OnMouseClicked -= Show;
        controller.OnMouseCanceled -= Hide;

        controller.OnItemFound += OnItemFound;
        controller.OnMouseClicked += Show;
        controller.OnMouseCanceled += Hide;

        Hide();
    }

    void OnItemFound(string info)
    {
        InfoText.text = info;
    }

    public override void Show()
    {
        base.Show();
    }

    public override void Hide()
    {
        base.Hide();
    }

    public override void Close()
    {
        controller.OnItemFound -= OnItemFound;
        controller.OnMouseClicked -= Show;
        controller.OnMouseCanceled -= Hide;
        base.Close();
    }
}

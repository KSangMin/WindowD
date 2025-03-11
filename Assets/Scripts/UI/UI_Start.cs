using UnityEngine;
using UnityEngine.UI;

public class UI_Start : UI
{
    public Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonclicked);   
    }

    void OnStartButtonclicked()
    {
        GameManager.Instance.player.SetCanLook(true);
        Close();
    }
}

using UnityEngine;

public class UI_Base : MonoBehaviour
{
    GameObject panel;

    private void Awake()
    {
        panel = transform.GetChild(0).gameObject;
    }

    public virtual void Show()
    {
        panel.SetActive(true);
    }

    public virtual void Hide()
    {
        panel.SetActive(false);
    }

    public virtual void Close()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string displayName;
    public string description;

    public string InfoText => "<E>키를 눌러 문 열기";

    public void Interact()
    {
        UIManager.Instance.ShowUI<UI_InteractDialogue>().ShowDialogue(description);
        //문이 열리는 메서드
    }
}

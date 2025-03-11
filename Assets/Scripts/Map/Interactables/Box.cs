using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    public string displayName;
    public string description;

    public string InfoText => "<E>키를 눌러 상자 열기";

    public void Interact()
    {
        UIManager.Instance.ShowUI<UI_InteractDialogue>().ShowDialogue(description);
        //상자를 열고 보상을 얻는 메서드
    }
}

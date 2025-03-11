using UnityEngine;

public class Box : MonoBehaviour, IInteractable
{
    public string displayName;
    public string description;

    public string InfoText => "<E>Ű�� ���� ���� ����";

    public void Interact()
    {
        UIManager.Instance.ShowUI<UI_InteractDialogue>().ShowDialogue(description);
        //���ڸ� ���� ������ ��� �޼���
    }
}

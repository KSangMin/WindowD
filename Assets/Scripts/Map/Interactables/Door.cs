using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public string displayName;
    public string description;

    public void Interact()
    {
        UIManager.Instance.ShowUI<UI_InteractDialogue>().ShowDialogue(description);
    }
}

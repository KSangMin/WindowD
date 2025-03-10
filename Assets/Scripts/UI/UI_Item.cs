using UnityEngine;
using UnityEngine.UI;

public class UI_Item : UI
{
    public Image itemIcon;

    private void Start()
    {
        ClearItem();

        GameManager.Instance.player.OnItemChanged -= SetItem;

        GameManager.Instance.player.OnItemChanged += SetItem;
    }

    void SetItem(ItemData item)
    {
        itemIcon.sprite = item.icon;
    }

    void ClearItem()
    {
        itemIcon.sprite = null;
    }
}

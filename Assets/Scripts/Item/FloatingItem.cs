using UnityEngine;

public abstract class FloatingItem : MonoBehaviour
{
    public ItemData itemData;

    Transform itemSprite;
    SpriteRenderer itemSR;
    float rotSpeed = 50f;

    protected void Awake()
    {
        itemSprite = transform.GetChild(0);
        itemSR = itemSprite.GetComponentInChildren<SpriteRenderer>();
        itemSR.sprite = itemData.icon;
    }

    protected void Update()
    {
        RotateImage();
    }

    void RotateImage()
    {
        itemSprite.Rotate(new Vector3(0, Time.deltaTime * rotSpeed, 0));
    }

    public abstract void ApplyItem();
}

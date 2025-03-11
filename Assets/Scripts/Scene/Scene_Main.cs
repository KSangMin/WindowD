using UnityEngine;

public class Scene_Main : Scene
{
    protected override void Init()
    {
        base.Init();

        UIManager.Instance.ShowUI<UI_Start>();
        UIManager.Instance.ShowUI<UI_Status>();
        UIManager.Instance.ShowUI<UI_Info>().Hide();
        UIManager.Instance.ShowUI<UI_Item>();
        UIManager.Instance.ShowUI<UI_DamageIndicator>().Hide();
        UIManager.Instance.ShowUI<UI_InteractDialogue>().Hide();
    }

    public override void Clear()
    {
        base.Clear();
    }
}

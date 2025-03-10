using UnityEngine;

public class Scene_Main : Scene
{
    protected override void Init()
    {
        base.Init();

        UIManager.Instance.ShowUI<UI_Status>();
    }

    public override void Clear()
    {
        base.Clear();
    }
}

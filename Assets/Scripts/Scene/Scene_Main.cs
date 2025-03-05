using UnityEngine;

public class Scene_Main : Scene_Base
{
    protected override void Init()
    {
        base.Init();

        Util.InstantiatePrefab("UI/EventSystem");
        UIManager.Instance.ShowUI<UI_Status>();
    }

    public override void Clear()
    {
        base.Clear();
    }
}

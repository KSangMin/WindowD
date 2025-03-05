using UnityEngine;
public class Scene_Base : MonoBehaviour
{
    protected void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        UIManager.Instance.Clear();
    }

    public virtual void Clear()
    {

    }
}

using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    List<UI_Base> _sceneUIs = new List<UI_Base>();
    Transform _root;
    Transform Root
    {
        get
        {
            if(_root == null)
            {
                _root = new GameObject("@UI_Root").transform;
            }
            return _root;
        }
    }

    public T ShowUI<T>() where T : UI_Base
    {
        string uiName = typeof(T).Name;
        T ui = Util.InstantiatePrefabAndGetComponent<T>(path: $"UI/{uiName}", parent: Root);
        _sceneUIs.Add(ui);

        return ui;
    }

    public void RemoveUI(UI_Base ui)
    {
        _sceneUIs.Remove(ui);
        ui.Close();
    }

    public void RemoveAllUI()
    {
        foreach (UI_Base ui in _sceneUIs)
        {
            UI_Base temp = ui;
            _sceneUIs.Remove(ui);
            temp.Close();
        }
    }

    public void Clear()
    {
        RemoveAllUI();
        Destroy(Root.gameObject);
        _root = null;
    }
}

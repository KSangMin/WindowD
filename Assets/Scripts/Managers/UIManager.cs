using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    Dictionary<Type, UI> _sceneDict = new Dictionary<Type, UI>();
    Stack<UI_Popup> _popupUIs = new Stack<UI_Popup>();

    int popupOrder = 10;

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

    public T ShowPopupUI<T>() where T : UI_Popup
    {
        Type uiType = typeof(T);

        T ui = Util.InstantiatePrefabAndGetComponent<T>(path: $"UI/{uiType.Name}", parent: Root);
        ui.SetCanvasOrder(popupOrder++);
        _popupUIs.Push(ui);

        return ui;
    }

    public void RemovePopupUI()
    {
        if (_popupUIs.Count <= 0) return;

        UI_Popup popup = _popupUIs.Pop();
        Destroy(popup.gameObject);
        popupOrder--;
        return;
    }

    public T HideUI<T>() where T : UI
    {
        Type uiType = typeof(T);

        if (_sceneDict.TryGetValue(uiType, out UI existingUI))
        {
            existingUI.Hide();
            return existingUI as T;
        }

        T ui = Util.InstantiatePrefabAndGetComponent<T>(path: $"UI/{uiType.Name}", parent: Root);
        _sceneDict[uiType] = ui;
        ui.Hide();

        return null;
    }

    public T ShowUI<T>(Transform par) where T : UI
    {
        if(par == null) return ShowUI<T>();

        Type uiType = typeof(T);

        if (_sceneDict.TryGetValue(uiType, out UI existingUI))
        {
            existingUI.Show();
            return existingUI as T;
        }

        T ui = Util.InstantiatePrefabAndGetComponent<T>(path: $"UI/{uiType.Name}", parent: par);
        _sceneDict[uiType] = ui;

        return ui;
    }

    public T ShowUI<T>() where T : UI
    {
        return ShowUI<T>(Root);
    }

    public void RemoveUI<T>() where T: UI
    {
        Type uiType = typeof(T);

        if (_sceneDict.TryGetValue(uiType, out UI existingUI))
        {
            _sceneDict.Remove(uiType);
            existingUI.Destroy();
            return;
        }
        else throw new InvalidOperationException($"There's No {uiType.Name} in UIManager");
    }

    public void RemoveUI(UI ui)
    {
        Type uiType = ui.GetType();

        if (_sceneDict.TryGetValue(uiType, out UI existingUI))
        {
            _sceneDict.Remove(uiType);
            existingUI.Destroy();
            return;
        }
        else throw new InvalidOperationException($"There's No {uiType.Name} in UIManager");
    }

    public void RemoveAllUI()
    {
        foreach (UI ui in _sceneDict.Values)
        {
            ui.Destroy();
        }

        _sceneDict.Clear();

        while(_popupUIs.Count > 0)
        {
            RemovePopupUI();
        }
    }

    public void Clear()
    {
        RemoveAllUI();
        Destroy(Root.gameObject);
        _root = null;
    }
}

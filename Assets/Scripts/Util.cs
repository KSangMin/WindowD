using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : MonoBehaviour
    {
        return go.TryGetComponent<T>(out var component) ?  component : go.AddComponent<T>();
    }
}

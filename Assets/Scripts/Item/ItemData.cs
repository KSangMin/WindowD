using UnityEngine;

[CreateAssetMenu(fileName = "New ItemData", menuName = "Create New ItemData")]
public class ItemData : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public Sprite icon;
}

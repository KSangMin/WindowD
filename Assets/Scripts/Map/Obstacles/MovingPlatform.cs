using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject platform;

    [SerializeField] private Transform first;
    [SerializeField] private Transform second;

    public float speed;

    private void Update()
    {
        float t = (1 - Mathf.Cos(Time.time * speed * Mathf.PI)) / 2f;

        platform.transform.position = Vector3.Lerp(first.position, second.position, t);
    }
}

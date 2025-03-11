using Unity.VisualScripting;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public Transform laserStartTransform;
    public Transform laserEndTransform;

    bool isDetected;
    float detectTime;
    public float detectRate = 1f;

    public float damage;

    private void Update()
    {
        if(!isDetected && !GameManager.Instance.player.isInvincible) CheckPlayer();
        if (isDetected)
        {
            detectTime -= Time.deltaTime;
            if(detectTime <= 0 )isDetected = false;
        }
    }

    void CheckPlayer()
    {
        Vector3 dir = laserEndTransform.position - laserStartTransform.position;
        Ray ray = new Ray(laserStartTransform.position, dir);
        Debug.DrawRay(ray.origin, ray.direction, Color.red, 0.1f);
        if(Physics.Raycast(ray, out RaycastHit hit, dir.magnitude))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!isDetected) 
                {
                    UIManager.Instance.ShowUI<UI_DamageIndicator>().Hitted();
                    GameManager.Instance.player.condition.TakeDamage(damage);
                }
                isDetected = true;
                detectTime = detectRate;
            }
        }
    }
}

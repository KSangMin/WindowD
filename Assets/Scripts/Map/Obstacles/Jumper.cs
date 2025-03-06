using System.Collections;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public float jumpPower = 160f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach(ContactPoint point in collision.contacts)
            {
                float cos = Vector3.Dot(point.normal, transform.up);
                if (cos < -0.5f)
                {
                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                    rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                    break;
                }
            }
            
        }
    }
}

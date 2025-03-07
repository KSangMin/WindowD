using System.Collections;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    public float jumpPower = 160f;
    bool hasJumped;

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasJumped && collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint point in collision.contacts)
            {
                float cos = Vector3.Dot(point.normal, transform.up);
                if (cos < -0.5f)
                {
                    Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                    rb.velocity = Vector3.zero;
                    rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
                    Debug.Log("มกวมวิ");
                    hasJumped = true;
                    break;
                }
            }
            //Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            //rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasJumped = false;
        }
    }
}

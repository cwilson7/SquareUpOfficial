using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookDetector : MonoBehaviour
{
    public GraplingHook parent;
    private Rigidbody rb;
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            parent.Hooked(transform.position);
        }
    }
    private void FixedUpdate()
    {
         rb.AddForce(new Vector3(0, -50, 0));
    }
}

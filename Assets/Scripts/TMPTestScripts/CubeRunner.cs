using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRunner : MonoBehaviour
{
    // Update is called once per frame
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.velocity = new Vector3(Input.GetAxis("Horizontal") * 10, rb.velocity.y, 0);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 20, 0);
        } 
    }


}

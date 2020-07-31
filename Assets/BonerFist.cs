using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonerFist : MonoBehaviour
{    
    private Transform Origin;
    float maxRadiusPunch = 5f, punchReturnRadius = 0.5f;//, maxRadiusBounce, minRadiusBounce, originalMaxBounce = 1f, withinOrigin, speedBoundary = 4f;
    float followSpeed = 10f;
    float punchSpeed = 20f;
    bool punching = false, returning = false;
    Vector3 mousePos;
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Origin = transform.parent;
    }

    private void FixedUpdate()
    {
        // ArtificialSpring();
        TrackMouse();
        if (Input.GetMouseButtonDown(0)) Punch();
        if (punching) PunchHandler(); 
        else DelayedFollow();
        HandleRotation();
    }

    void HandleRotation()
    {
        float rotateSpeed = 500f;
        Quaternion desiredRotation;
        Quaternion defaultRotation = Quaternion.Euler(90, 90, 90);
        float defaultRotationRadius = 0.5f;
        
        if ((Origin.position - transform.position).magnitude < defaultRotationRadius)
        {
            desiredRotation = defaultRotation;
        }
        else
        {
            float angle = Vector3.Angle(Vector3.down, transform.position);
            if (transform.position.x >= Origin.position.x)
            {
                desiredRotation = Quaternion.Euler(90 - angle, 90, 90);
            }
            else
            {
                desiredRotation = Quaternion.Euler(90 + angle, 90, 90);
            }
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
    }

    void TrackMouse()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        MouseWorldPos.z = transform.position.z;
        mousePos = (MouseWorldPos - transform.position).normalized;
        Debug.Log(mousePos);
    }

    void DelayedFollow()
    {
        rb.velocity = (Origin.position - transform.position) * followSpeed;
    }

    void PunchHandler()
    {
        if ((Origin.position - transform.position).magnitude > maxRadiusPunch) ReturnFist();
        if (returning)
        {
            ReturnFist();
            if ((Origin.position - transform.position).magnitude < punchReturnRadius)
            {
                punching = false;
                returning = false;
            }
        }
    }

    void Punch()
    {
        punching = true;
        Vector3 direction = new Vector3(mousePos.x, mousePos.y, 0f);
        rb.velocity = direction * punchSpeed;
    }

    void ReturnFist()
    {
        returning = true;
        Vector3 direction = (Origin.position - transform.position).normalized;
        rb.velocity = direction * punchSpeed;
    }

    //DEPRECATED
    void ArtificialSpring()
    {
        /*
        float distance = (Origin.position - transform.position).magnitude;
        Vector3 direction = (Origin.position - transform.position).normalized;
        if (distance > maxRadiusBounce)
        {
           // if (insideBounceBoundary)
            DelayedFollow();
            minRadiusBounce = originalMaxBounce - 0.5f;
        }
        else
        {
            insideBounceBoundary = true;
            if (rb.velocity.magnitude < speedBoundary)
            {
                rb.velocity = direction * rb.velocity.magnitude;
            }
            else if (rb.velocity.normalized != direction && distance > minRadiusBounce)
            {
                rb.velocity = -(rb.velocity.magnitude / 2) * direction;
                minRadiusBounce = minRadiusBounce / 2;
            }
        }
        */
    }

}

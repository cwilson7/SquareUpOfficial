using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonerFist : MonoBehaviour
{    
    private Transform Origin;
    float maxRadiusPunch = 5f, punchReturnRadius = 0.5f;//, maxRadiusBounce, minRadiusBounce, originalMaxBounce = 1f, withinOrigin, speedBoundary = 4f;
    float followSpeed = 5f;
    float punchSpeed = 10f;
    public bool punching = false, returning = false, redirecting = false;
    Vector3 mousePos, savedDirection;
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
        if (Input.GetMouseButtonDown(0)) Punch(new Vector3(mousePos.x, mousePos.y, 0f));
        if (punching) PunchHandler();
        else DelayedFollow();

        if (!redirecting) HandleRotation();
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
            desiredRotation = Direct(Origin.position, transform.position, Direction.FromCenter);
        }

        if (!punching) transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
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
        if (redirecting)
        {
            //send to origin
            SendToOrigin();
            if ((Origin.position - transform.position).magnitude < punchReturnRadius) { 
                Punch(savedDirection);
                redirecting = false;
            }
        }
        else
        {
            if ((Origin.position - transform.position).magnitude > maxRadiusPunch) ReturnFist();
            if (returning)
            {
                ReturnFist();
                if ((Origin.position - transform.position).magnitude < punchReturnRadius)
                {
                    punching = false;
                    returning = false;
                    savedDirection = Vector3.zero;
                }
            }
        }
    }

    void SendToOrigin()
    {
        Vector3 vec = Origin.position - transform.position;
        rb.velocity = vec * punchSpeed * 10;
        rb.rotation = Direct(Origin.position, transform.position, Direction.ToCenter);
    }

    void Punch(Vector3 direction)
    {
        punching = true;
        if ((Origin.position - transform.position).magnitude > punchReturnRadius)
        {
            redirecting = true;
            savedDirection = direction;
        }
        else
        {
            rb.velocity = direction * punchSpeed;
            rb.rotation = Direct(Origin.position, direction, Direction.FromCenter);
        }
    }

    Quaternion Direct (Vector3 origin, Vector3 point, Direction direction)
    {
        Quaternion retQuat;
        float angle = Vector3.Angle(Vector3.down, point);
        if (point.x > origin.x)
        {
            if (direction == Direction.FromCenter) retQuat = Quaternion.Euler(90 - angle, 90, 90);
            else retQuat = Quaternion.Euler(90 + angle, 90, 90);
        }
        else
        {
            if (direction == Direction.FromCenter) retQuat = Quaternion.Euler(90 + angle, 90, 90);
            else retQuat = Quaternion.Euler(90 - angle, 90, 90);
        }
        return retQuat;
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

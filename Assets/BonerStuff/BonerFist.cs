using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonerFist : MonoBehaviour
{    
    private Transform Origin;
    float maxRadiusPunch = 5f, punchReturnRadius = 0.5f;
    float followSpeed = 5f;
    float punchSpeed = 10f;
    public bool punching = false, returning = false, redirecting = false, following = true;
    public Vector3 mousePos;
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
        rb.rotation = Quaternion.LookRotation(-Origin.position, Vector3.up);
        if (Input.GetMouseButtonDown(0)) Punch(new Vector3(mousePos.x, mousePos.y, 0f));
        if (Input.GetMouseButtonDown(1)) following = !following;
        if (punching) PunchHandler();
        else if (following) DelayedFollow();

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
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(-Origin.position + transform.position, Vector3.up);
        }

        if (!punching) transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
    }

    void TrackMouse()
    {
        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        MouseWorldPos.z = transform.position.z;
        mousePos = (MouseWorldPos - transform.position).normalized;
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

    void SendToOrigin()
    {
        Vector3 vec = Origin.position - transform.position;
        rb.velocity = vec.normalized * punchSpeed * 10;
    }

    void Punch(Vector3 direction)
    {
        punching = true;
        rb.velocity = direction * punchSpeed;
        rb.rotation = Quaternion.LookRotation(-Origin.position + transform.position, Vector3.up);
    }


    void ReturnFist()
    {
        returning = true;
        Vector3 direction = (Origin.position - transform.position).normalized;
        rb.velocity = direction * punchSpeed;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CustomUtilities;

public class Fist : DamageDealer
{
    Controller ParentController;
    SphereCollider collide;
    Rigidbody rb;

    public bool hasGun;

    Vector3 savedDirection;

    float rotateSpeed = 500f;
    Quaternion desiredRotation;
    Quaternion defaultRotation = Quaternion.Euler(90, 90, 90);
    float defaultRotationRadius = 0.5f;

    public Transform Origin;
    float maxRadiusPunch = 5f, punchReturnRadius = 2f;
    float followSpeed = 20f;
    float punchSpeed = 30f;
    public bool punching = false, returning = false, redirecting = false;

    private void FixedUpdate()
    {
        if (ParentController == null || !ParentController.controllerInitialized) return;
        if (punching) PunchHandler();
        if (!punching)
        {
            DelayedFollow();
            DirectionHandler();
        }
    }

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;
        collide = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;
        rb.useGravity = false;
        SetCollider(false);

        damage = ParentController.punchPower;
        impactMultiplier = ParentController.punchImpact;
        owner = ParentController.actorNr;
        gameObject.tag = "Fist";
    }

    void DelayedFollow()
    {
        rb.velocity = (Origin.position - transform.position) * followSpeed;
    }

    void DirectionHandler()
    {
        if ((Origin.position - transform.position).magnitude < defaultRotationRadius) desiredRotation = defaultRotation;
        else desiredRotation = Quaternion.LookRotation(-(Origin.position - transform.position), Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
    }

    void PunchHandler()
    {
        if (redirecting)
        {
            rb.velocity = (Origin.position - transform.position).normalized * punchSpeed * 1.5f;
            rb.rotation = Quaternion.LookRotation(Origin.position - transform.position, Vector3.up);
            if ((Origin.position - transform.position).magnitude < defaultRotationRadius)
            {
                redirecting = false;
                Punch(savedDirection);
            }
        }
        else
        {
            rb.rotation = Quaternion.LookRotation(- Origin.position + transform.position, Vector3.up);
            if ((Origin.position - transform.position).magnitude > maxRadiusPunch) ReturnFist();
            if (returning)
            {
                Vector3 direction = (Origin.position - transform.position).normalized;
                rb.velocity = direction * punchSpeed;
                if ((Origin.position - transform.position).magnitude < punchReturnRadius)
                {
                    punching = false;
                    returning = false;
                }
            }
        }
    }

    void ReturnFist()
    {
        returning = true;
        SetCollider(false);
        Vector3 direction = (Origin.position - transform.position).normalized;
        rb.velocity = direction * punchSpeed;
    }

    public void Punch(Vector3 aim)
    {
        if (!punching && (Origin.position - transform.position).magnitude > defaultRotationRadius)
        {
            savedDirection = aim;
            redirecting = true;
            punching = true;
        }
        else
        {
            punching = true;
            SetCollider(true);
            rb.velocity = new Vector3(aim.x, aim.y, 0f).normalized * punchSpeed + ParentController.gameObject.GetComponent<Rigidbody>().velocity;
            savedDirection = Vector3.zero;
        }
    }

    public void SetCollider(bool isActive) { collide.enabled = isActive; }
}

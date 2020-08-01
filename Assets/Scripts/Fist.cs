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

    float rotateSpeed = 300f;
    Quaternion desiredRotation;
    Quaternion defaultRotation = Quaternion.Euler(90, 90, 90);
    float defaultRotationRadius = 0.5f;

    public Transform Origin;
    float maxRadiusPunch = 5f, punchReturnRadius = 1f;
    float followSpeed = 20f;
    float punchSpeed = 30f;
    bool punching = false, returning = false;

    private void Update()
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
        Origin = transform.parent;
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
        if ((Origin.position - transform.position).magnitude < defaultRotationRadius)
        {
            desiredRotation = defaultRotation;
        }
        else
        {
            float angle = Vector3.Angle(Vector3.down, transform.localPosition);
            if (transform.position.x > Origin.position.x)
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

    void ReturnFist()
    {
        returning = true;
        SetCollider(false);
        Vector3 direction = (Origin.position - transform.position).normalized;
        rb.velocity = direction * punchSpeed;
    }

    public void Punch(Vector3 aim)
    {
        punching = true;
        SetCollider(true);
        Vector3 direction = new Vector3(aim.x, aim.y, 0f);
        rb.velocity = direction * punchSpeed + ParentController.gameObject.GetComponent<Rigidbody>().velocity;
        float angle = Vector3.Angle(Vector3.down, direction);
        if (direction.x > 0)//Origin.position.x)
        {
            rb.rotation = Quaternion.Euler(90 - angle, 90, 90);
        }
        else
        {
            rb.rotation = Quaternion.Euler(80 + angle, 90, 90);
        }
    }

    public void SetCollider(bool isActive) { collide.enabled = isActive; }
}

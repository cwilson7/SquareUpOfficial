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
    Quaternion defaultRotation; //= Quaternion.Euler(90, 90, 90);
    float defaultRotationRadius = 0.5f;

    public Transform Origin;
    float maxRadiusPunch = 5f, punchReturnRadius = 2f;
    float followSpeed = 40f;
    float punchSpeed = 30f;
    public bool punching = false, returning = false, redirecting = false;

    [Header("Cosmetic Information")]
    public int[] materialIndexesToChange;

    public float fistReturnSpeedMultiplier = 0.5f;
    private void Start()
    {
        defaultRotation = Quaternion.Euler(-transform.localRotation.eulerAngles.x, 90, 90);
    }

    private void FixedUpdate()
    {
        if (SceneManagerHelper.ActiveSceneBuildIndex != 2)
        {
           // DummyFistHandler();
        }
        if (ParentController == null || !ParentController.controllerInitialized) return;
        if (punching) PunchHandler();
        if (!punching)
        {
            DelayedFollow();
            DirectionHandler();
        }
    }

    void DummyFistHandler()
    {
        DelayedFollow();
        DirectionHandler();
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
        Vector3 veloicty, distance = Origin.position - transform.position;
        /*if (Mathf.Abs(distance.magnitude) > maxRadiusPunch) veloicty = distance * followSpeed * 2f;
        else */veloicty = distance * followSpeed;
        rb.velocity = veloicty;
    }

    public void InitializeDummy()
    {
        rb = GetComponent<Rigidbody>();
    }

    void DirectionHandler()
    {
        if ((Origin.position - transform.position).magnitude < defaultRotationRadius) desiredRotation = defaultRotation;
        else desiredRotation = Quaternion.LookRotation(-(Origin.position - transform.position), Vector3.up);

        //transform.localRotation = Quaternion.Euler(90, 0, 0);
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
            if ((Origin.position - transform.position).magnitude > maxRadiusPunch && !returning) ReturnFist();
            if (returning)
            {
                Vector3 velocity, returnVector = Origin.position - transform.position;
                velocity = returnVector * followSpeed * fistReturnSpeedMultiplier;
                rb.velocity = velocity;
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
        rb.velocity = direction * punchSpeed * ParentController.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
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

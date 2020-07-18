using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CustomUtilities;

public class Fist : DamageDealer
{
    Controller ParentController;
    SphereCollider collide;
    SpringJoint spring;
    Rigidbody rb;
    public float restSpringForce;
    public float punchSpringForce;
    public float punchForce;

    public Vector3 startLoc;

    public bool hasGun;

    private void Update()
    {
        if (ParentController == null || !ParentController.controllerInitialized) return;
        if (collide.enabled == false) startLoc = transform.position;
        if (hasGun) trackMouse(ParentController.AimDirection);

    }

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;
        collide = GetComponent<SphereCollider>();
        spring = transform.parent.GetComponent<SpringJoint>();
        rb = GetComponent<Rigidbody>();
        spring.connectedBody = rb;
        //if (transform.parent.GetComponent<LFist>() == null) spring.anchor = new Vector3(1.2f, 2f, -.55f);
        //else spring.anchor = new Vector3(-1.2f, 2f, -.55f);
        //spring.connectedAnchor = new Vector3(0, 0, -0.015f);
        //spring.connectedAnchor = gameObject.transform.position;
        SetCollider(false);

        damage = ParentController.punchPower;
        impactMultiplier = ParentController.punchImpact;
        owner = ParentController.actorNr;
        startLoc = transform.position;
        gameObject.tag = "Fist";
        restSpringForce = 50;
        punchSpringForce = 5;
        punchForce = 75;
        spring.spring = restSpringForce;
        hasGun = false;
    }

    public void Punch(Vector3 aim)
    {
        spring.spring = punchSpringForce;
        rb.useGravity = false;
        SetCollider(true);
        rb.AddForce(new Vector3(aim.x, aim.y,0f) * punchForce);
        StartCoroutine(FistDrag());
    }

    IEnumerator FistDrag()
    {
        yield return new WaitForSeconds(0.4f);
        rb.useGravity = true;
        spring.spring = restSpringForce;
        //rb.velocity = new Vector3(0, 0, 0);
        //transform.localPosition = spring.connectedAnchor;
        SetCollider(false);

    }

    public void SetHasGun(bool gun)
    {
        hasGun = gun;
        if (gun)
        {
            rb.useGravity = false;
            spring.spring = 0f;
        }
        else
        {
            rb.useGravity = true;
            spring.spring = restSpringForce;
        }
    }

    private void trackMouse(Vector3 aimDir)
    {
        //rb.position = aimDir*2;
        //rb.
        //rb.MovePosition(ParentController.currentWeapon.transform.position);
    }

    public void SetCollider(bool isActive) { collide.enabled = isActive; }
}

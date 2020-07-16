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

    public Vector3 startLoc;

    private void Update()
    {
        if (ParentController == null || !ParentController.controllerInitialized) return;
        if (collide.enabled == false) startLoc = transform.position;
    }

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;
        collide = GetComponent<SphereCollider>();
        spring = GetComponent<SpringJoint>();
        rb = GetComponent<Rigidbody>();
        spring.connectedBody = parentController.gameObject.GetComponent<Rigidbody>();
        if (transform.parent.GetComponent<LFist>() == null) spring.connectedAnchor = new Vector3(1.2f, 2f, -.55f);
        else spring.connectedAnchor = new Vector3(-1.2f, 2f, -.55f);
        //spring.connectedAnchor = gameObject.transform.position;
        SetCollider(false);

        damage = ParentController.punchPower;
        impactMultiplier = ParentController.punchImpact;
        owner = ParentController.actorNr;
        startLoc = transform.position;
        gameObject.tag = "Fist";
    }

    public void Punch(Vector3 aim)
    {

    }

    public void SetCollider(bool isActive) { collide.enabled = isActive; }
}

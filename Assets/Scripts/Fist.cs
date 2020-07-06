using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CustomUtilities;

public class Fist : DamageDealer
{
    Controller ParentController;
    SphereCollider collide;

    private float activeTime;
    public bool isActive;
    private bool currentState;
    public Vector3 startLoc;

    public void FixedUpdate()
    {
        if (gameObject.tag != "Fist") return;
        if (isActive != currentState) SetCollider(isActive); 

    }

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;
        //collide = gameObject.AddComponent<SphereCollider>();
        GameObject armature = Utils.FindParentWithClass<Armature>(transform).gameObject;
        //collide.radius = ParentController.fistRadius / armature.transform.localScale.magnitude;
        //collide.isTrigger = true;
        collide = GetComponent<SphereCollider>();
        SetCollider(false);
        currentState = false;

        damage = ParentController.punchPower;
        impactMultiplier = ParentController.punchImpact;
        owner = ParentController.actorNr;
        startLoc = transform.position;
        gameObject.tag = "Fist";
    }

    public void SetCollider(bool isActive)
    {
        collide.enabled = isActive;
        currentState = isActive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (collide == null) return;
        Gizmos.DrawWireSphere(transform.position, collide.radius * 10f);
    }
}

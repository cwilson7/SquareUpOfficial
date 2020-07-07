﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CustomUtilities;

public class Fist : DamageDealer
{
    Controller ParentController;
    SphereCollider collide;

    public Vector3 startLoc;

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;
        GameObject armature = Utils.FindParentWithClass<Armature>(transform).gameObject;
        collide = GetComponent<SphereCollider>();
        SetCollider(false);

        damage = ParentController.punchPower;
        impactMultiplier = ParentController.punchImpact;
        owner = ParentController.actorNr;
        startLoc = transform.position;
        gameObject.tag = "Fist";
    }

    public void SetCollider(bool isActive) { collide.enabled = isActive; }
}

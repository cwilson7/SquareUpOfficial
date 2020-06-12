﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fist : DamageDealer
{
    Controller ParentController;
    SphereCollider Collider;

    public float cooldown, timeBtwnPunches;
    public Transform fistLocation;
    private float activeTime;
    private Vector3 PrevPos;

    public void FixedUpdate()
    {
        if (cooldown >= 0) cooldown -= Time.deltaTime;
        if (gameObject.tag != "Fist") return;
    }

    public void Punch(Vector3 dir)
    {
        SetCollider(true);
        Velocity = new Vector3(dir.x, dir.y, 0f) * impactMultiplier;
        cooldown = timeBtwnPunches;
        StartCoroutine(FistDrag());
    }

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;
        Collider = gameObject.AddComponent<SphereCollider>();
        Collider.radius = ParentController.fistRadius;
        Collider.isTrigger = true;
        SetCollider(false);

        damage = ParentController.punchPower;
        impactMultiplier = ParentController.punchImpact;
        owner = ParentController.actorNr;
        cooldown = ParentController.punchCooldown;
        timeBtwnPunches = cooldown;
        activeTime = ParentController.fistActiveTime;
        PrevPos = transform.position;
        gameObject.tag = "Fist";
    }

    public IEnumerator FistDrag()
    {
        yield return new WaitForSeconds(activeTime);
        SetCollider(false);
    }

    public void SetCollider(bool isActive)
    {
        Collider.enabled = isActive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (Collider == null) return;
        Gizmos.DrawWireSphere(transform.position, Collider.radius);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using CustomUtilities;

public class Fist : DamageDealer
{
    Controller ParentController;
    SphereCollider collide;

    public float cooldown, timeBtwnPunches;
    public Transform fistLocation;
    private float activeTime;
    private Vector3 PrevPos;

    public void FixedUpdate()
    {
        if (cooldown >= 0) cooldown -= Time.deltaTime;
        if (gameObject.tag != "Fist") return;
    }

    public void Punch()
    {
        SetCollider(true);
        cooldown = timeBtwnPunches;
        StartCoroutine(FistDrag());
    }

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;
        collide = gameObject.AddComponent<SphereCollider>();
        GameObject armature = Utils.FindParentWithClass<Armature>(transform).gameObject;
        collide.radius = ParentController.fistRadius / armature.transform.localScale.magnitude;
        collide.isTrigger = true;
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
        collide.enabled = isActive;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (collide == null) return;
        Gizmos.DrawWireSphere(transform.position, collide.radius * 10f);
    }
}

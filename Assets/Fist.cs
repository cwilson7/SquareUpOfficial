using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fist : MonoBehaviour
{
    Controller ParentController;
    public int owner;
    public float damage, impact, cooldown, timeBtwnPunches;

    private void Awake()
    {
        ParentController = GetComponentInParent<Controller>();
    }
    public void Update()
    {
        if (cooldown >= 0) cooldown -= Time.deltaTime;
    }
    public void Smack(Vector3 Direction)
    {
        if (cooldown > 0) return;
        ParentController.gameObject.GetComponent<PhotonView>().RPC("RPC_MeleAttack", RpcTarget.AllBuffered, Direction, owner);
    }

    public void InitializeFist()
    {
        GetComponent<SphereCollider>().enabled = false;

        damage = ParentController.punchPower;
        impact = ParentController.punchImpact;
        owner = ParentController.actorNr;
        cooldown = ParentController.punchCooldown;
        timeBtwnPunches = cooldown;

        GetComponent<MeshRenderer>().sharedMaterial = LobbyController.lc.availableMaterials[LobbyController.lc.selectedMaterialIDs[owner - 1]];
    }

    public IEnumerator FistDrag()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Rigidbody>().velocity = -GetComponent<Rigidbody>().velocity;
        yield return new WaitForSeconds(0.5f);
        transform.localPosition = new Vector3(0, 0, 0);
        GetComponent<SphereCollider>().enabled = false;
    }
}

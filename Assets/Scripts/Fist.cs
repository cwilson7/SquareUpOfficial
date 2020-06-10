using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fist : MonoBehaviour
{
    Controller ParentController;
    public int owner;
    public float damage, impact, cooldown, timeBtwnPunches;
    public Transform fistLocation;

    public void Update()
    {
        if (cooldown >= 0) cooldown -= Time.deltaTime;
        //transform.position = fistLocation.position;
    }
    public void Smack(Vector3 Direction)
    {
        if (cooldown > 0) return;
        GameInfo.GI.StatChange(owner, "punchesThrown");
        //ParentController.gameObject.GetComponent<PhotonView>().RPC("RPC_MeleAttack", RpcTarget.AllBuffered, Direction, owner);
    }

    public void InitializeFist()
    {
        ParentController = GetComponentInParent<Controller>();
        GetComponent<SphereCollider>().enabled = false;

        damage = ParentController.punchPower;
        impact = ParentController.punchImpact;
        owner = ParentController.actorNr;
        cooldown = ParentController.punchCooldown;
        timeBtwnPunches = cooldown;

        transform.localPosition = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);

        GetComponent<MeshRenderer>().sharedMaterial = LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(owner).CustomProperties["AssignedColor"]];
    }

    public IEnumerator FistDrag()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        transform.localPosition = new Vector3(0, 0, 0);
        GetComponent<SphereCollider>().enabled = false;
    }
}

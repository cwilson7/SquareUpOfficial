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
    public Vector3 Velocity;
   
    private Vector3 OriginalPos, PrevPos;
    private float maxTimePunch, punchDistance;

    public void FixedUpdate()
    {
        if (cooldown >= 0) cooldown -= Time.deltaTime;
        TrackVelocity();
    }
    public void Smack(Vector3 Direction)
    {
        if (cooldown > 0) return;
        GameInfo.GI.StatChange(owner, "punchesThrown");
        ParentController.gameObject.GetComponent<PhotonView>().RPC("RPC_MeleAttack", RpcTarget.AllBuffered, Direction, owner);
    }
    
    public IEnumerator Punch(Vector3 Direction)
    {
        //turn on collider
        Vector3 punchLoc = Direction.normalized * punchDistance;
        punchLoc.z = 0f;
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / maxTimePunch;
            transform.position = Vector3.Lerp(transform.position, transform.position + punchLoc, t);             
            yield return null;
        }
        StartCoroutine(Return(punchLoc));
    }

    IEnumerator Return(Vector3 PunchLoc)
    {
        //turn off collider 
        //float quicker = 2f;
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / maxTimePunch ;
            transform.position = Vector3.Lerp(transform.position, transform.position - (PunchLoc ), t);
            yield return null;
        }
    }

    private void TrackVelocity()
    {
        Velocity = (transform.position - OriginalPos) / Time.deltaTime;
        OriginalPos = transform.position;
    }

    public void InitializeFist(Controller parentController)
    {
        ParentController = parentController;

        damage = ParentController.punchPower;
        impact = ParentController.punchImpact;
        owner = ParentController.actorNr;
        cooldown = ParentController.punchCooldown;
        timeBtwnPunches = cooldown;
        punchDistance = ParentController.punchDistance;
        maxTimePunch = ParentController.maxTimePunch;

        OriginalPos = transform.position;

       //GetComponent<MeshRenderer>().sharedMaterial = LobbyController.lc.availableMaterials[(int)PhotonNetwork.CurrentRoom.GetPlayer(owner).CustomProperties["AssignedColor"]];
    }

    public IEnumerator FistDrag()
    {
        yield return new WaitForSeconds(0.5f);
        //GetComponent<SphereCollider>().enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}

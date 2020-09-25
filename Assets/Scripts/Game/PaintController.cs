using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PaintController : MonoBehaviour
{
    private PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        Controller.OnDamgeTaken += SplatterPaint;
    }

    void SplatterPaint(DamageDealer projInfo, Controller player)
    {
        //checks to see if the collision is far from player, which shows that the player has respawned away from the explosion
        //if its too far, dont do anything
        if (Mathf.Abs((projInfo.gameObject.transform.position - player.transform.position).magnitude) > 10f) return;
        
        int attackerActorNumber = projInfo.owner;
        int attackerMatID = (int)PhotonNetwork.CurrentRoom.GetPlayer(attackerActorNumber).CustomProperties["AssignedColor"];
        int damagedActorNumber = player.actorNr;
        
        Vector3 paintDirection;
        if (projInfo.gameObject.GetComponent<Rigidbody>() == null) paintDirection = projInfo.Velocity.normalized;
        else paintDirection = projInfo.gameObject.GetComponent<Rigidbody>().velocity.normalized;

        paintDirection.z += 0.65f;
        if (paintDirection.y < 0) paintDirection.y += 0.25f;
        PV.RPC("SplatterPaint_RPC", RpcTarget.AllBuffered, attackerMatID, damagedActorNumber, paintDirection);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    [PunRPC]
    public void SplatterPaint_RPC(int matID, int damagedID, Vector3 paintDirection)
    {
        Score damagedInfo = (Score)GameInfo.GI.scoreTable[damagedID];
        Material matOfProj = LobbyController.lc.availableMaterials[matID];
        ParticleSystem ps = damagedInfo.playerAvatar.GetComponent<Controller>().PaintExplosionSystem;
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(matOfProj.GetColor("_Color"));
        ps.transform.rotation = Quaternion.LookRotation(paintDirection, Vector3.up);

        ps.Play();
    }
}

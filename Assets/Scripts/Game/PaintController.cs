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
        int attackerActorNumber = projInfo.owner;
        int attackerMatID = (int)PhotonNetwork.CurrentRoom.GetPlayer(attackerActorNumber).CustomProperties["AssignedColor"];
        int damagedActorNumber = player.actorNr;
        float impactMultiplier = projInfo.impactMultiplier;
        Vector3 projVelocity = projInfo.Velocity.normalized;
        Vector3 InitialPaintVelocity = projVelocity * impactMultiplier;
        PV.RPC("SplatterPaint_RPC", RpcTarget.AllBuffered, attackerMatID, damagedActorNumber, InitialPaintVelocity);
    }

    [PunRPC]
    public void SplatterPaint_RPC(int matID, int damagedID, Vector3 PaintVelocity)
    {
        Score damagedInfo = (Score)GameInfo.GI.scoreTable[damagedID];
        Material matOfProj = LobbyController.lc.availableMaterials[matID];
        ParticleSystem ps = damagedInfo.playerAvatar.GetComponent<Controller>().PaintExplosionSystem;
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(matOfProj.GetColor("_Color"));
        main.startSize = PaintVelocity.magnitude / 3;

        ps.emissionRate = 40f;
        ps.Play();
    }
}

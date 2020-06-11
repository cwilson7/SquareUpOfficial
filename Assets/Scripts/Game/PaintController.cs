using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Runtime.InteropServices;

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
        int AttackerMatID = (int)PhotonNetwork.CurrentRoom.GetPlayer(attackerActorNumber).CustomProperties["AssignedMaterial"];
        int damagedActorNumber = player.actorNr;
        float impactMultiplier = projInfo.impactMultiplier;
        Vector3 projVelocity = projInfo.Velocity.normalized;
        Vector3 InitialPaintVelocity = projVelocity * impactMultiplier;
        PV.RPC("SplatterPaint_RPC", RpcTarget.AllBuffered, attackerActorNumber, damagedActorNumber, InitialPaintVelocity);
    }

    [PunRPC]
    public void SplatterPaint_RPC(int matID, int damagedID, Vector3 PaintVelocity)
    {
        Score damagedInfo = (Score)GameInfo.GI.scoreTable[damagedID];
        Material matOfProj = LobbyController.lc.availableMaterials[matID];

        GameObject paintEffect = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/PaintEffect"), damagedInfo.playerAvatar.transform.position, Quaternion.identity);
        Paint paint = paintEffect.GetComponent<Paint>();
        paint.Initialize(PaintVelocity, matOfProj);
    }
}

using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public abstract void ItemAbility(int actorNr);
    public abstract void PickUpEffect(Transform transform);

    public int id;

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        PickUp(other.GetComponent<PhotonView>().OwnerActorNr);
    }

    protected void PickUp(int actorNr)
    {
        if (actorNr != PhotonNetwork.LocalPlayer.ActorNumber) return;
        PickUpEffect(transform);
        GameInfo.GI.StatChange(actorNr, "powerUpsCollected");
        ItemAbility(actorNr);
        GameManager.Manager.PV.RPC("DestroyPowerUp_RPC", RpcTarget.AllBuffered, id);
    }
}

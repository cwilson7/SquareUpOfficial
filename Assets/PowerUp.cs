using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    protected PhotonView PV;

    public abstract void ItemAbility(int actorNr);
    public abstract void PickUpEffect(Transform transform);

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("touching something ;)");
        if (other.tag != "Player") return;
        PickUp(other.GetComponent<PhotonView>().OwnerActorNr);
    }

    protected void PickUp(int actorNr)
    {
        PickUpEffect(transform);
        GameInfo.GI.StatChange(actorNr, "powerUpsCollected");
        ItemAbility(actorNr);
        PV.RPC("DestroyPowerUp_RPC", RpcTarget.AllBuffered, PV.ViewID);
    }

    [PunRPC]
    public void DestroyPowerUp_RPC(int viewID)
    {
        GameObject pwrUp = PhotonView.Find(viewID).gameObject;
        Destroy(pwrUp);
    }
}

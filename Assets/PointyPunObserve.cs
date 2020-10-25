using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointyPunObserve : MonoBehaviour, IPunObservable
{
    PhotonView PV;
    public float netAbilityVelocity;
    public Vector3 netTransformUp;
    PointyController parentController;
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (parentController == null) return;
        Debug.Log("serialize view being called for actor: " + PV.OwnerActorNr);
        
        if (stream.IsWriting)
        {
            if (parentController.rocketMode) stream.SendNext(parentController.rocketVelocity);
            stream.SendNext(transform.transform.up);
        }
        else if (stream.IsReading)
        {
            if (parentController.rocketMode) netAbilityVelocity = (float)stream.ReceiveNext();
            netTransformUp = (Vector3)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    public void InitializePointyAbilityPun(PointyController _parent)
    {
        parentController = _parent;
        PV = _parent.PV;
        PV.ObservedComponents.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (PV == null || parentController == null || PV.IsMine) return;

        parentController.rocketVelocity = netAbilityVelocity;

        if (parentController.rocketMode)
        {
            var step = parentController.rotateSpeed * Time.deltaTime;
            transform.up = Vector3.Lerp(transform.up, netTransformUp, step);
        }
    }
}

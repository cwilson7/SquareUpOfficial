using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkAvatar : MonoBehaviourPun, IPunObservable
{
    private PhotonView PV;
    protected Vector3 remotePlayerPosition;

    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            remotePlayerPosition = (Vector3)stream.ReceiveNext();

            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine) return;
        
        Vector3 lagDistance = remotePlayerPosition - transform.position;
        double timeToReachGoal = currentPacketTime - lastPacketTime;
        currentTime += Time.deltaTime;
        if(lagDistance.magnitude > 5f)
        {
            transform.position = remotePlayerPosition;
        }
        else 
        {
            transform.position = Vector3.Lerp(positionAtLastPacket, remotePlayerPosition, (float)(currentTime / timeToReachGoal));
        }
    }
}

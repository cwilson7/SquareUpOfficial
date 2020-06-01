using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkAvatar : MonoBehaviourPun, IPunObservable
{
    private PhotonView PV;
    protected Vector3 remotePlayerPosition;

    CharacterController cc;
    Controller controller;

    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
        controller = GetComponent<Controller>();
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

        CheckForMove(lagDistance, timeToReachGoal);
        cc.Move((controller.Velocity * controller.speed + controller.impact * 10f) * Time.deltaTime);
    }

    private void CheckForMove(Vector3 lagDistance, double timeToReachGoal)
    {
        if (timeToReachGoal < 1.0 && Mathf.Abs(lagDistance.y) > 0.11f && controller.jumpNum > 0)
        {
            controller.Jump();
        }
        
        if (Mathf.Abs(lagDistance.x) < 0.11f)
        {
            controller.Velocity.x = 0f;
        }
        else
        {
            if (lagDistance.x > 0)
            {
                controller.Velocity.x = 1f;
            }
            else if (lagDistance.x < 0)
            {
                controller.Velocity.x = -1f;
            }
        }
    }
}

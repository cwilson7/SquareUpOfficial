using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class NetworkAvatar : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float MinimumLagDistance;
    
    private PhotonView PV;
    protected Vector3 remotePlayerPosition;
    protected Vector3 remotePlayerVelocity;

    CharacterController cc;
    Controller controller;

    Vector3 NetworkedVelocity = Vector3.zero;

    float currentTime = 0;
    double currentPacketTime = 0;
    double lastPacketTime = 0;
    Vector3 positionAtLastPacket = Vector3.zero;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
        //controller = GetComponent<Controller>();
    }

    private void SetController()
    {
        controller = GetComponent<Controller>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            if (controller != null) stream.SendNext(controller.Velocity);
        }
        else
        {
            remotePlayerPosition = (Vector3)stream.ReceiveNext();
            if (controller != null) remotePlayerVelocity = (Vector3)stream.ReceiveNext();

            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PV.IsMine || GetComponent<Controller>() == null) return;
        if (controller == null) SetController();
        if (!controller.controllerInitialized) return;
        else RecieveNetworkMovement();
    }

    private void RecieveNetworkMovement()
    {
        Vector3 LagDistance = remotePlayerPosition - transform.position;
        if (LagDistance.magnitude > 5f)
        {
            transform.position = remotePlayerPosition;
        }
        /*
        //NetworkedVelocity.y = controller.Velocity.y;
        if (Mathf.Abs(LagDistance.x) < MinimumLagDistance)
        {
            NetworkedVelocity.x = 0;
        }
        else {
            if (LagDistance.x > 0)
            {
                //move to right
                NetworkedVelocity.x = 1;
            }
            else
            {
                //move to left
                NetworkedVelocity.x = -1;
            }
        }
        //just track y
        transform.position = new Vector3(transform.position.x, remotePlayerPosition.y, remotePlayerPosition.z);
        */
        controller.Move(remotePlayerVelocity);//(NetworkedVelocity);
    }
        
        
        /*
        Vector3 lagDistance = remotePlayerPosition - transform.position;
        double timeToReachGoal = currentPacketTime - lastPacketTime;
        currentTime += Time.deltaTime;
        if (lagDistance.magnitude > 5f)
        {
            transform.position = remotePlayerPosition;
        }

        CheckForMove(lagDistance, timeToReachGoal);
        cc.Move((controller.Velocity * controller.speed + controller.impact * 10f) * Time.deltaTime);
        controller.impact = Vector3.Lerp(controller.impact, Vector3.zero, 5 * Time.deltaTime);
    }

    private void CheckForMove(Vector3 lagDistance, double timeToReachGoal)
    {
        
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
    */
}

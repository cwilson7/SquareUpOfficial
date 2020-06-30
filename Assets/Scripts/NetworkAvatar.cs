using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Controller))]
[AddComponentMenu("Photon Networking/Photon Character Controller View")]

public class NetworkAvatar : MonoBehaviourPun, IPunObservable
{
    private float m_Distance;

    private CharacterController myCC;

    private Controller m_controller;

    private PhotonView PV;

    private Vector3 m_NetworkPosition;

    private Quaternion m_NetworkRotation;

    public bool m_SynchronizeVelocity = true;

    public bool m_TeleportEnabled = false;
    public float m_TeleportIfDistanceGreaterThan = 3.0f;

    public void Awake()
    {
        this.myCC = GetComponent<CharacterController>();
        this.PV = GetComponent<PhotonView>();

        this.m_NetworkPosition = new Vector3();
        this.m_NetworkRotation = new Quaternion();
    }

    public void FixedUpdate()
    {
        if (m_controller == null)
        {
            SetController();
            return;
        }
        if (!m_controller.controllerInitialized) return;
        if (!this.PV.IsMine)
        {
            this.myCC.transform.rotation = this.m_NetworkRotation;//Quaternion.RotateTowards(this.myCC.transform.rotation, this.m_NetworkRotation, 1.0f / PhotonNetwork.SerializationRate);
            this.myCC.transform.position = Vector3.MoveTowards(this.myCC.transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.myCC.transform.position);
            stream.SendNext(this.myCC.transform.rotation);

            if (this.m_SynchronizeVelocity)
            {
                stream.SendNext(this.myCC.velocity);
            }
        }
        else
        {
            this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
            this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();

            if (this.m_TeleportEnabled)
            {
                if (Vector3.Distance(this.myCC.transform.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
                {
                    this.myCC.transform.position = this.m_NetworkPosition;
                }
            }

            if (this.m_SynchronizeVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                //set velocity
                if (m_controller != null) this.m_controller.Velocity = (Vector3)stream.ReceiveNext();

                this.m_NetworkPosition += this.myCC.velocity * lag;

                this.m_Distance = Vector3.Distance(this.myCC.transform.position, this.m_NetworkPosition);
            }
        }
    }

    private void SetController()
    {
        this.m_controller = GetComponent<Controller>();
    }
}
    
    
    /*
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
        controller.Move(remotePlayerVelocity);//(NetworkedVelocity);
    }
    */

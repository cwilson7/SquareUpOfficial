using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;


[RequireComponent(typeof(PhotonView))]
//[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(Controller))]
[AddComponentMenu("Photon Networking/Photon Character Controller View")]

public class NetworkAvatar : MonoBehaviourPun, IPunObservable
{
    private float m_Distance;

    //private CharacterController myCC;
    private Rigidbody myCC;

    private Controller m_controller;

    private Vector3 controllerVelocity;

    private PhotonView PV;

    private Vector3 m_NetworkPosition;

    private Quaternion m_NetworkRotation;

    public bool m_SynchronizeVelocity = true;

    public bool m_TeleportEnabled = false;
    public float m_TeleportIfDistanceGreaterThan = 3.0f;

    public void Awake()
    {
        //this.myCC = GetComponent<CharacterController>();
        this.myCC = GetComponent<Rigidbody>();
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
            if (controllerVelocity.x != 0)
            {
                m_controller.FreezePositons(false);
            }
            else m_controller.FreezePositons(true);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.myCC.transform.position);
            stream.SendNext(this.myCC.transform.rotation);
            if (m_controller != null) stream.SendNext(m_controller.Velocity);

            if (this.m_SynchronizeVelocity)
            {
                stream.SendNext(this.myCC.velocity);
            }
        }
        else
        {
            this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
            this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            if (m_controller != null) controllerVelocity = (Vector3)stream.ReceiveNext();

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
                if (m_controller != null) myCC.velocity = (Vector3)stream.ReceiveNext();//this.m_controller.Velocity = (Vector3)stream.ReceiveNext();

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
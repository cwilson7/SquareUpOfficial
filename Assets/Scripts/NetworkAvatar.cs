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

    //private CharacterController myRB;
    private Rigidbody myRB;

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
        //this.myRB = GetComponent<CharacterController>();
        this.myRB = GetComponent<Rigidbody>();
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
            this.myRB.transform.rotation = this.m_NetworkRotation;//Quaternion.RotateTowards(this.myRB.transform.rotation, this.m_NetworkRotation, 1.0f / PhotonNetwork.SerializationRate);
            //this.myRB.transform.position = Vector3.MoveTowards(this.myRB.transform.position, this.m_NetworkPosition, this.m_Distance * (1.0f / PhotonNetwork.SerializationRate));
            //this.myRB.velocity()
            SetVelocity();
            if (controllerVelocity.x != 0)
            {
                m_controller.FreezePositions(false);
            }
            else m_controller.FreezePositions(true);
        }
    }

    void SetVelocity()
    {
        Vector3 direction = (m_NetworkPosition - transform.position).normalized;
        myRB.velocity = new Vector3(direction.x * m_controller.speed, myRB.velocity.y, 0f);
        if (Mathf.Abs(m_NetworkPosition.y - transform.position.y) > 30)
        {
            myRB.MovePosition(m_NetworkPosition);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this.myRB.transform.position);
            stream.SendNext(this.myRB.transform.rotation);
            if (m_controller != null) stream.SendNext(m_controller.Velocity);

            if (this.m_SynchronizeVelocity)
            {
                //stream.SendNext(this.myRB.velocity);
            }
        }
        else
        {
            this.m_NetworkPosition = (Vector3)stream.ReceiveNext();
            this.m_NetworkRotation = (Quaternion)stream.ReceiveNext();
            if (m_controller != null) controllerVelocity = (Vector3)stream.ReceiveNext();

            if (this.m_TeleportEnabled)
            {
                if (Vector3.Distance(this.myRB.transform.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
                {
                    this.myRB.transform.position = this.m_NetworkPosition;
                }
            }

            if (this.m_SynchronizeVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                //set velocity
                //if (m_controller != null) myRB.velocity = (Vector3)stream.ReceiveNext();//this.m_controller.Velocity = (Vector3)stream.ReceiveNext();

                //this.m_NetworkPosition += this.myRB.velocity * lag;

                //this.m_Distance = Vector3.Distance(this.myRB.transform.position, this.m_NetworkPosition);
            }
        }
    }

    private void SetController()
    {
        this.m_controller = GetComponent<Controller>();
    }
}
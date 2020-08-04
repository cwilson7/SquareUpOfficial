using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]

public class NetworkAvatar : MonoBehaviourPun, IPunObservable
{
    private Controller m_controller;

    private PhotonView PV;

    public bool reacted = false;

    public void Awake()
    {
        this.PV = GetComponent<PhotonView>();
    }

    public void FixedUpdate()
    {
        if (m_controller == null)
        {
            SetController();
            return;
        }
        if (!m_controller.controllerInitialized) return;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }

    private void SetController()
    {
        this.m_controller = GetComponent<Controller>();
    }
}
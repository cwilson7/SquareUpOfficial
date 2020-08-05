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
    Rigidbody rb;

    private PhotonView PV;

    bool netRunning, netHasGun;

    public void Awake()
    {
        this.PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        if (m_controller == null)
        {
            SetController();
            return;
        }
        if (!m_controller.controllerInitialized) return;
        if (PV.IsMine) return;
    }

    void ImpactHandler()
    {
        rb.velocity += m_controller.impact;
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
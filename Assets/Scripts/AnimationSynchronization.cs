using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.ComponentModel.Design;

public class AnimationSynchronization : MonoBehaviour, IPunObservable
{
    //this will be on playertestavatar
    //observed by PV
    private PhotonView PV;
    private Controller controller;
    private Animator animator;
    private Vector3 aim;
    private int directionModifier;
    private bool isRunning, hasGun, jumping, meleeing, specialing;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (GetComponent<Controller>() == null || !GetComponent<Controller>().controllerInitialized) return;
        if (controller == null) SetController();
        if (PV.IsMine) return;
        GhostAnimate(aim, isRunning, hasGun);
    }

    private void SetController()
    {
        controller = GetComponent<Controller>();
        animator = controller.anim;
    }

    private void GhostAnimate(Vector3 aim, bool running, bool gun)
    {
        animator.SetFloat("AimX", aim.x * directionModifier);
        animator.SetFloat("AimY", aim.y);
        animator.SetBool("Running", running);
        animator.SetBool("Gun", gun);
        //PV.RPC("SyncTriggers_RPC", RpcTarget.AllBuffered,melee,jump,special);
    }
    //what needs to happen for animations
    //set float for aimx, aimy
    //Set bool running, bool for gun
    //set trigger jump or melee
    //set triggers will happen in RPC functions
    //bool and float will be set onserialization

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (controller == null) return;
        if(stream.IsWriting)
        {
            stream.SendNext(controller.AimDirection);
            stream.SendNext(controller.isRunning);
            stream.SendNext(controller.hasGun);
            stream.SendNext(controller.directionModifier);
        }
        else if (stream.IsReading)
        {
            aim = (Vector3)stream.ReceiveNext();
            isRunning = (bool)stream.ReceiveNext();
            hasGun = (bool)stream.ReceiveNext();
            directionModifier = (int)stream.ReceiveNext();
        }
    }
    [PunRPC]
    private void SyncTriggers_RPC(bool melee, bool jump, bool special)
    {
        if (melee)
        {
            animator.SetTrigger("Melee");
        }
        if (jump)
        {
            animator.SetTrigger("Jump");
        }
        if (special)
        {
            animator.SetTrigger("Special");
        }
    }
}

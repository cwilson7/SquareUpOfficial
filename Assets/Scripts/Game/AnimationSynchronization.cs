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
    private Rigidbody rb;
    public Vector3 aim;
    public int directionModifier;
    private bool isRunning, hasGun;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
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
        hasGun = controller.hasGun;
        
        Vector3 oldAim = new Vector3(animator.GetFloat("AimX"), animator.GetFloat("AimY"), 0f);
        float distance = Vector3.Distance(oldAim, aim);
        Vector3 smoothAim = Vector3.MoveTowards(oldAim, aim, distance * (1.0f / PhotonNetwork.SerializationRate));
        
        animator.SetFloat("AimX", smoothAim.x * directionModifier);
        animator.SetFloat("AimY", smoothAim.y);
        animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));
        animator.SetBool("Gun", gun);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (controller == null) return;
        if(stream.IsWriting)
        {
            stream.SendNext(controller.AimDirection);
        }
        else if (stream.IsReading)
        {
            aim = (Vector3)stream.ReceiveNext();
        }
    }
}

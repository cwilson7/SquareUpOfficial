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
    public Vector3 aim;
    public int directionModifier;
    public bool flinched;
    private bool isRunning, hasGun, jumping, specialing, hitRight,hitLeft;
    private int melee;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (GetComponent<Controller>() == null || !GetComponent<Controller>().controllerInitialized) return;
        if (controller == null) SetController();
        if (PV.IsMine) return;
        GhostAnimate(aim, isRunning, hasGun, melee);
    }

    public void SetFlinch()
    {
        flinched = true;
        StartCoroutine(FlinchTimer());
    }

    IEnumerator FlinchTimer()
    {
        yield return new WaitForSeconds(3 * 1f / PhotonNetwork.SerializationRate);
        flinched = false;
    }

    private void SetController()
    {
        controller = GetComponent<Controller>();
        animator = controller.anim;
    }

    private void GhostAnimate(Vector3 aim, bool running, bool gun, int melee)
    {
        Vector3 oldAim = new Vector3(animator.GetFloat("AimX"), animator.GetFloat("AimY"), 0f);
        float distance = Vector3.Distance(oldAim, aim);
        Vector3 smoothAim = Vector3.MoveTowards(oldAim, aim, distance * (1.0f / PhotonNetwork.SerializationRate));
        animator.SetFloat("AimX", smoothAim.x * directionModifier);
        animator.SetFloat("AimY", smoothAim.y);
        animator.SetBool("Running", running);
        animator.SetBool("Gun", gun);
    }

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
    private void SyncTriggers_RPC(bool jump, bool special)
    {
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

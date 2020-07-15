using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimtionEventHandler : MonoBehaviour
{
    Controller parentController;
    public float meleeSlow;
    public AudioSource ads;
    // Start is called before the first frame update
    public void InitializeEventHandler(Controller pc)
    {
        parentController = pc;
        meleeSlow = 0.35f;
        //ads = GameObject.Find("AudioManager");
    } 

    // Update is called once per frame
    public void FlinchStart()
    {
        //parentController.blockInput = true;
    }
    public void FlinchEnd()
    {
        //parentController.blockInput = false;
    }
    public void MeleeStart()
    {
        parentController.speed /= 4;
        parentController.PV.RPC("AnimationSetPosition_RPC", RpcTarget.All, transform.position);
        StartCoroutine(MeleeSlow(meleeSlow));
    }

    public void JumpStart()
    {
        //parentController.PV.RPC("AnimationSetPosition_RPC", RpcTarget.All, transform.position);
    }

    public void JumpEnd()
    {
        //parentController.PV.RPC("AnimationSetPosition_RPC", RpcTarget.All, transform.position);
        Debug.Log("end of jump");
    }

    private IEnumerator MeleeSlow(float delay)
    {
        yield return new WaitForSeconds(delay);
        parentController.speed *= 4;
    }
}

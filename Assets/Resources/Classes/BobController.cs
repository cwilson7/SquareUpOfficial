using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BobController : Controller
{
    public GraplingHook hook;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        hook = gameObject.GetComponentInChildren<GraplingHook>();
        hook.parentController = this;
    }

    public override void SpecialAbility()
    {
        anim.SetTrigger("Special");
        hook.aimDirection.x = AimDirection.x;
        hook.aimDirection.y = AimDirection.y;
        hook.aimDirection.z = 0f; //AimDirection.z;
        PV.RPC("FireWarpMissile_RPC", RpcTarget.AllBuffered, hook.aimDirection);
    }

    public void Warp(Vector3 place)
    {
        //cc.enabled = false;
        //cc.gameObject.transform.position = place;
        //cc.enabled = true;
        //Velocity.y = 0;
        //jumpNum = maxJumps;
    }

    [PunRPC]
    public void FireWarpMissile_RPC(Vector3 Direction)
    {
        hook.Fire(Direction);
    }
}

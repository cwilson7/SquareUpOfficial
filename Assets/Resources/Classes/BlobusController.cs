using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobusController : Controller
{
    public int warpDistance;
    public float warpDelay;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        warpDistance = 5;
        warpDelay = 0.1f;
    }

    public override void SpecialAbility()
    {
        anim.SetTrigger("Special");
        StartCoroutine(SpecialTimer(warpDelay));
    }

    IEnumerator SpecialTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        Warp();
    }

    public void Warp()
    {
        //cc.enabled = false;
        //AimDirection.z = 0;
        //cc.gameObject.transform.position = cc.gameObject.transform.position + AimDirection*warpDistance;
        //cc.enabled = true;
        //Velocity.y = 0;
        //jumpNum = maxJumps;
    }
}

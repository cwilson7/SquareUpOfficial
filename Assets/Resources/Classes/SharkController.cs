using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SharkController : Controller
{
    public bool IsInSpecial;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        IsInSpecial = false;
    }

    public override void SpecialAbility()
    {
        IsInSpecial = true;
        _Collider.enabled = false;
        anim.SetBool("Special", true);
        speed = 20;
        StartCoroutine(SpecialTimer());
    }

    public override void Movement()
    {
        if (!IsInSpecial) HandleInputs(iPhone);

        Move(Velocity);
    }

    public override void Gravity()
    {
        if (!IsInSpecial) base.Gravity();
    }

    IEnumerator SpecialTimer()
    {
        yield return new WaitForSeconds(5);
        if (IsInSpecial)
        {
            _Collider.enabled = true;
            IsInSpecial = false;
            anim.SetBool("Special", false);
            PV.RPC("Jaw_RPC", RpcTarget.AllBuffered,actorNr);
        }
    }

    private void ExitSpecial()
    {
        _Collider.enabled = true;
        IsInSpecial = false;
        anim.SetBool("Special", false);
        PV.RPC("Jaw_RPC", RpcTarget.AllBuffered,actorNr);
    }

    [PunRPC]
    public void Jaw_RPC(int actNum)
    {
        GameObject jaw = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + "SharkJaw"), transform.position - new Vector3(0,0,0), Quaternion.Euler(0,180,0));
        jaw.GetComponent<Damager>().setValues(100, 5, actNum,0.25f);
    }

}

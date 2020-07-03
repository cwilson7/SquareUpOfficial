using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class YetiController : Controller
{
    public float specialForce;
    public bool isInSpecial;
    public override void InitializePlayerController()
    {
        base.InitializePlayerController();
        specialForce = 5f;
    }

    public override void SpecialAbility()
    {
        isInSpecial = true;
        anim.SetBool("Special", true);
        Vector3 dir = AimDirection;
        dir.z = 0;
        PV.RPC("Avalanche_RPC", RpcTarget.AllBuffered,dir);
        if (AimDirection.x > 0) gameObject.transform.localRotation = Quaternion.Euler(Mathf.Rad2Deg * Mathf.Atan(-AimDirection.y / AimDirection.x), 90, -90);
        else gameObject.transform.localRotation = Quaternion.Euler(180 + Mathf.Rad2Deg * Mathf.Atan(-AimDirection.y / AimDirection.x), 90, -90);
        Velocity.y = AimDirection.y*specialForce;
        Velocity.x = AimDirection.x*specialForce;
        StartCoroutine(SpecialTimer());
        Move(Velocity);
    }

    [PunRPC]
    public void Avalanche_RPC(Vector3 AimDir)
    {
        if (AimDir.x > 0)
        {
            GameObject avalanche = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + "YetiBend"), transform.position, Quaternion.Euler(new Vector3(0, 0, 90+Mathf.Rad2Deg * Mathf.Atan(AimDir.y / AimDir.x))));
            avalanche.GetComponent<YetiBend>().aimDirection = AimDir;
        }
        else
        {
            GameObject avalanche = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + "YetiBend"), transform.position, Quaternion.Euler(new Vector3(0, 0, 270 + Mathf.Rad2Deg * Mathf.Atan(AimDir.y / AimDir.x))));
            avalanche.GetComponent<YetiBend>().aimDirection = AimDir;
        }
    }

    IEnumerator SpecialTimer()
    {
        yield return new WaitForSeconds(.25f);
        isInSpecial = false;
        anim.SetBool("Special", false);
    }

    public override void Movement()
    {
       if (!isInSpecial) HandleInputs(iPhone);

        Move(Velocity);
    }

    public override void Gravity()
    {
        if (!isInSpecial)
        {
            base.Gravity();
        }
    }
}

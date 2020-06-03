using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private PhotonView PV;
    private Controller ParentController;

    public float damage, fireRate, fireCooldown, bltSpeed, recoil;
    public float impact;
    public int owner, totalAmmo, ammoLeft;
    public GameObject projectile;
    public Transform FiringPoint, GunPivot;

    private void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
        ParentController = GetComponentInParent<Controller>();

        ammoLeft = totalAmmo;
        fireCooldown = 0f;
    }

    private void FixedUpdate()
    {
        if (fireCooldown >= 0) fireCooldown -= Time.deltaTime;
    }

    private void RotateGun(Vector2 angle)
    {
        Vector2 ang = angle.normalized;
        if (ang.x < 0) transform.localEulerAngles = new Vector3(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan(ang.y / ang.x));
        else transform.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan(ang.y / ang.x));

    }

    public void Attack(Vector3 Direction)
    {
        if (fireCooldown > 0) return;
        
        GameInfo.GI.StatChange(owner, "bulletsFired");

        Direction = Direction.normalized;

        ParentController.impact += Vector3.Normalize(new Vector3(-Direction.x, -Direction.y, 0)) * recoil;
        PV.RPC("FireWeapon_RPC", RpcTarget.AllBuffered, Direction, damage, impact, bltSpeed, owner, projectile.name);

        fireCooldown = fireRate;
        ammoLeft -= 1;
        if (ammoLeft < 1)
        {
            PV.RPC("LoseWeapon_RPC", RpcTarget.AllBuffered);
        }
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }

}

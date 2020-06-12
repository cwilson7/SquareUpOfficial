using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;

public class Weapon : MonoBehaviour
{
    private PhotonView PV;
    private Controller ParentController;

    public float damage, fireRate, fireCooldown, bltSpeed, recoil;
    public float impact;
    public int owner, totalAmmo, ammoLeft;
    public GameObject projectile;
    public Transform FiringPoint, GunPivot, GunLocation;

    private void Start()
    {
        GameObject parentAvatar = Utils.FindParentWithClass<Controller>(transform).gameObject;
        PV = parentAvatar.GetComponent<PhotonView>();
        ParentController = parentAvatar.GetComponent<Controller>();
        GunPivot = GetComponentInParent<GunPivot>().gameObject.transform;
        ammoLeft = totalAmmo;
        fireCooldown = 0f;
    }

    private void FixedUpdate()
    {
        if (fireCooldown >= 0) fireCooldown -= Time.deltaTime;
        TrackMousePosition(ParentController.AimDirection);
        GunPivot.position = GunLocation.position;
    }

    public void TrackMousePosition(Vector3 Direction)
    {
        if (!PV.IsMine) return;
        //Debug.Log("W "+GunPivot.transform.parent.position); 
        //Debug.Log("L "+GunPivot.transform.parent.localPosition);
        //GunPivot.transform.position = GunPivot.transform.parent.TransformPoint(GunPivot.transform.parent.position);
        transform.localRotation = Quaternion.Euler(0f, 0f, transform.localRotation.z);
        if (Direction.x > 0) GunPivot.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan(Direction.y / Direction.x));
        else GunPivot.rotation = Quaternion.Euler(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan(Direction.y / Direction.x));
    }

    public virtual void Attack(Vector3 Direction)
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

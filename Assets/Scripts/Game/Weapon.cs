using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;

public class Weapon : MonoBehaviour
{
    private PhotonView PV;
    private Controller ParentController;
    public GameObject parentAvatar;
    private AnimationSynchronization AS;

    public float damage, fireRate, fireCooldown, bltSpeed, recoil;
    public float impact;
    public int owner, totalAmmo, ammoLeft;
    public GameObject projectile;
    public Transform FiringPoint, GunPivot, GunLocation;
    public int currentMod = 0;

    public Vector3 networkedRotation;

    public void InitalizeWeapon()
    {
        parentAvatar = ((Score)GameInfo.GI.scoreTable[owner]).playerAvatar;
        PV = parentAvatar.GetComponent<PhotonView>();
        ParentController = parentAvatar.GetComponent<Controller>();
        AS = ParentController.GetComponentInChildren<AnimationSynchronization>();
        ammoLeft = totalAmmo;
        fireCooldown = 0f;
    }

    private void FixedUpdate()
    {
        if (fireCooldown >= 0) fireCooldown -= Time.deltaTime;
    }

    private void Update()
    {
        if (PV.IsMine) TrackMousePosition();
        if (!PV.IsMine) NetworkTracking();
    }

    private void NetworkTracking()
    {
        networkedRotation = ParentController.GetComponent<NetworkAvatar>().netAim;
        transform.position = GunPivot.position;
        NetSetTransform(networkedRotation);
    }

    public void TrackMousePosition()//Vector3 Direction, bool directionCheck, bool lerp)
    {
        if (ParentController.AimDirection.x < 0) SetTransform(0, 0, -180 - Mathf.Rad2Deg * Mathf.Atan(ParentController.AimDirection.y / -ParentController.AimDirection.x));
        else SetTransform(0, 0, - Mathf.Rad2Deg * Mathf.Atan(ParentController.AimDirection.y / -ParentController.AimDirection.x));
    }

    private void NetSetTransform(Vector3 netAim)
    {
        float speed = 15;
        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(netAim), speed * Time.deltaTime);
    }

    private void SetTransform(float x, float y, float z)
    {
        networkedRotation = new Vector3(x, y, z);
        transform.position = GunPivot.position;
        transform.rotation = Quaternion.Euler(x, y, z);
    }

    public virtual void Attack(Vector3 Direction)
    {
        if (fireCooldown > 0) return;
        
        GameInfo.GI.StatChange(owner, Stat.bulletsFired);

        Direction = Direction.normalized;

        ParentController.impact += Vector3.Normalize(new Vector3(-Direction.x, -Direction.y, 0)) * recoil;
        PV.RPC("FireWeapon_RPC", RpcTarget.AllBuffered, Direction, damage, impact, bltSpeed, owner, projectile.name, transform.rotation.eulerAngles);

        fireCooldown = fireRate;
        ammoLeft -= 1;
        if (ammoLeft < 1)
        {
            PV.RPC("LoseWeapon_RPC", RpcTarget.AllBuffered);
        }

        PhotonNetwork.SendAllOutgoingCommands();
    }

    public void Remove()
    {
        Destroy(this.gameObject);
    }
}

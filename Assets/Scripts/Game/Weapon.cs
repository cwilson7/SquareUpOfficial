using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtilities;

public class Weapon : MonoBehaviour
{
    private PhotonView PV;
    private Controller ParentController;
    private AnimationSynchronization AS;

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
        AS = ParentController.GetComponentInChildren<AnimationSynchronization>();
        ammoLeft = totalAmmo;
        fireCooldown = 0f;
    }

    private void FixedUpdate()
    {
        if (fireCooldown >= 0) fireCooldown -= Time.deltaTime;
        if (PV.IsMine) TrackMousePosition(ParentController.AimDirection, ParentController.directionModifier == 1, false);
        else TrackMousePosition(AS.aim, AS.directionModifier == 1, true);
    }

    public void TrackMousePosition(Vector3 Direction, bool directionCheck, bool lerp)
    {
        if (directionCheck)
        {
            if (Direction.x > 0) SetTransform(0, -90, Mathf.Rad2Deg * Mathf.Atan(Direction.y / Direction.x), lerp);
            else SetTransform(0, -90, 180 + Mathf.Rad2Deg * Mathf.Atan(Direction.y / Direction.x), lerp);
        }
        else
        {
            if (Direction.x > 0) SetTransform(0, -90, 180 + Mathf.Rad2Deg * Mathf.Atan(-Direction.y / Direction.x), lerp);
            else SetTransform(0, -90, Mathf.Rad2Deg * Mathf.Atan(-Direction.y / Direction.x), lerp);
        }
    }

    private void SetTransform(float x, float y, float z, bool lerp)
    {
        if (!lerp)
        {
            transform.position = GunLocation.position;
            transform.localRotation = Quaternion.Euler(x, y, z);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, GunLocation.position, 1.0f / PhotonNetwork.SerializationRate);
            Quaternion desiredRotation = Quaternion.Euler(x, y, z);
            float angle = Quaternion.Angle(transform.rotation, desiredRotation);
            transform.localRotation = Quaternion.RotateTowards(Quaternion.Euler(transform.localRotation.eulerAngles), desiredRotation, angle * (1.0f / PhotonNetwork.SerializationRate));
        }
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

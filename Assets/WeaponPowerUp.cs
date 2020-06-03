using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponPowerUp : PowerUp
{
    public GameObject WeaponPrefab;
    
    public override void ItemAbility(int actorNr)
    {
        InitializeWeapon(actorNr);
    }

    public void InitializeWeapon(int actor)
    {
        PV.RPC("EquipWeapon_RPC", RpcTarget.AllBuffered, actor);
    }

    [PunRPC]
    public void EquipWeapon_RPC(int ownr)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[ownr];
        GameObject wpn = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/"+WeaponPrefab.name), playerInfo.playerAvatar.transform.position, Quaternion.identity);
        wpn.transform.SetParent(playerInfo.playerAvatar.GetComponentInChildren<GunPivot>().transform);
        wpn.GetComponent<Weapon>().owner = ownr;
        playerInfo.playerAvatar.GetComponent<Controller>().currentWeapon = wpn.GetComponent<Weapon>();
    }

    public override void PickUpEffect(Transform transform)
    {
        // effects
    }
}

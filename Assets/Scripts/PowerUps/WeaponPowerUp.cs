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
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actor];
        Weapon playerWeapon = playerInfo.playerAvatar.GetComponent<Controller>().currentWeapon;
        if (playerWeapon != null) playerInfo.playerAvatar.GetComponent<Controller>().PV.RPC("LoseWeapon_RPC", RpcTarget.AllBuffered);
        playerInfo.playerAvatar.GetComponent<Controller>().anim.SetBool("Gun", true);
        GameManager.Manager.PV.RPC("EquipWeapon_RPC", RpcTarget.AllBuffered, actor, WeaponPrefab.name);
    }

    public override void PickUpEffect(Transform transform)
    {
        // effects
    }
}

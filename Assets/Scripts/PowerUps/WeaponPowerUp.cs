﻿using System.Collections;
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
        GameManager.Manager.PV.RPC("EquipWeapon_RPC", RpcTarget.AllBuffered, actor, WeaponPrefab.name);
    }

    public override void PickUpEffect(Transform transform)
    {
        // effects
    }
}
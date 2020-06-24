﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public PhotonView PV;
    public static GameManager Manager;
    
    //Set values
    [SerializeField] private double percentOfPowerUpsWeapons;
    [SerializeField] private float powerUpMaxCooldown;
    [SerializeField] private int maxPowerups = ;

    //Tracked values
    [SerializeField] private float powerUpCooldown;
    [SerializeField] public bool gameStarted = false;
    [SerializeField] private Hashtable currentPowerUps = new Hashtable();
    private System.Random rand;
    
    // Start is called before the first frame update
    void Awake()
    {
        Manager = this;
        PV = GetComponent<PhotonView>();
        rand = new System.Random();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameStarted) return;

        HandleTimers();
        HandlePowerUps();
    }

    public void InitalizeGameManager()
    {
        //Set Initial Values
        powerUpCooldown = powerUpMaxCooldown;
        PV.RPC("GameStart_RPC", RpcTarget.AllBuffered);
    }

    private void HandleTimers()
    {
        if (currentPowerUps.Count >= maxPowerups || GameInfo.GI.TimeStopped) return;
        if (powerUpCooldown >= 0) powerUpCooldown -= Time.deltaTime;
    }

    #region Power Up Stuff
    private void HandlePowerUps()
    {
        if (powerUpCooldown < 0f)
        {
            if (RandomPercent() <= percentOfPowerUpsWeapons)
            {
                //instantiate weapon power up
                PowerUpSelection(1);
            }
            else
            {
                //instantiate power up
                PowerUpSelection(2);
            }
            powerUpCooldown = powerUpMaxCooldown;
        }
    }

    private void PowerUpSelection(int idOfList)
    {
        List<GameObject> pwrUpList = GameInfo.GI.ReturnListFromID(idOfList);
        Transform[] pwrUpLocs = Cube.cb.CurrentFace.ReturnArrayFromID(idOfList);
        int newID = GenerateIDForPowerUp();
        int locID = RandomInteger(0, pwrUpLocs.Length);
        //if locid has something generate new random number
        int itemID = RandomInteger(0, pwrUpList.Count);
        PV.RPC("InstantiatePowerUp_RPC", RpcTarget.AllBuffered, idOfList, locID, itemID, newID);
    }

    private int GenerateIDForPowerUp()
    {
        int id = RandomInteger(0, 100);
        if (currentPowerUps.ContainsKey(id))
        {
            return GenerateIDForPowerUp();
        }
        else return id;
    }

    private double RandomPercent()
    {
        return rand.NextDouble();
    }

    private int RandomInteger(int min, int max)
    {
        return rand.Next(max);
    }

    public void DestroyAllPowerUps()
    {
        Debug.Log("length of power up list: " + currentPowerUps.Count);
        int keyLength = currentPowerUps.Count;
        List<int> keys = new List<int>();
        foreach (int key in currentPowerUps.Keys) keys.Add(key);
        for (int i = 0; i < keyLength; i++)
        {
            PV.RPC("DestroyPowerUp_RPC", RpcTarget.AllBuffered, keys[i]);
        }
    }

    #endregion

    #region RPCs
    [PunRPC]
    public void InstantiatePowerUp_RPC(int listID, int locID, int pwrUpID, int newID)
    {
        List<GameObject> pwrUpList = GameInfo.GI.ReturnListFromID(listID);
        Transform[] pwrUpLocs = Cube.cb.CurrentFace.ReturnArrayFromID(listID);
        GameObject pwrUp = Instantiate(pwrUpList[pwrUpID], pwrUpLocs[locID]);
        pwrUp.GetComponent<PowerUp>().id = newID;
        currentPowerUps.Add(newID, pwrUp);
    }

    [PunRPC]
    public void DestroyPowerUp_RPC(int id)
    {
        if (!currentPowerUps.ContainsKey(id)) return;
        
        GameObject pwrUp = (GameObject)currentPowerUps[id];
        Destroy(pwrUp);
        currentPowerUps.Remove(id);
    }

    [PunRPC]
    public void EquipWeapon_RPC(int actor, string weaponName)
    {
        Score playerInfo = (Score)GameInfo.GI.scoreTable[actor];
        GameObject wpn = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/Weapons/" + weaponName), playerInfo.playerAvatar.GetComponentInChildren<GunPivot>().transform.position, Quaternion.identity);
        wpn.transform.SetParent(playerInfo.playerAvatar.GetComponentInChildren<GunPivot>().transform);
        wpn.GetComponent<Weapon>().owner = actor;
        wpn.GetComponent<Weapon>().GunLocation = playerInfo.playerAvatar.GetComponentInChildren<GunLocation>().transform;
        playerInfo.playerAvatar.GetComponent<Controller>().currentWeapon = wpn.GetComponent<Weapon>();
    }

    [PunRPC]
    public void GameStart_RPC()
    {
        gameStarted = true;
    }

    #endregion
}

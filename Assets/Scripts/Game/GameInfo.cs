﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using JetBrains.Annotations;

public class GameInfo : MonoBehaviour
{
    public static GameInfo GI;
    public PhotonView PV;
    public Hashtable scoreTable;
    public bool TimeStopped, cubeCloned = false;
    private bool allReady;
    public Cube CubeClone;
    public int cubeOwner;
    public List<GameObject> WeaponPowerUps, PowerUps;
    public List<RuntimeAnimatorController> AnimatorControllers;

    private bool started = false, setScoreTable = false, stopUpdateCalls = false;

    [SerializeField] private GameObject scorePrefab;

    // Start is called before the first frame update
    void Awake()
    {
        GI = this;
        scoreTable = new Hashtable();
    }

    public void InitializeGameInfo()
    {
        PV = GetComponent<PhotonView>();
        TimeStopped = true;
        started= false;
        CubeClone = CopyCube(Cube.cb);
        cubeCloned = true;
        PopulateList<GameObject>(WeaponPowerUps, "PhotonPrefabs/PowerUps/WeaponPowerUps");
        PopulateList<GameObject>(PowerUps, "PhotonPrefabs/PowerUps/OtherPowerUps");
        PopulateList<RuntimeAnimatorController>(AnimatorControllers, "PhotonPrefabs/AnimatorControllers");
        InitializeScoreTable();
    }

    public Cube CopyCube(Cube original)
    {
        Cube clone = new Cube();
        clone.InstantiatedLevelIDs = original.InstantiatedLevelIDs;
        clone.DistanceFromCameraForRotation = original.DistanceFromCameraForRotation;
        clone.inRotation = original.inRotation;
        clone.cubeRot = original.cubeRot;
        clone.currFaceID = original.currFaceID;
        clone.ownerActorNr = original.ownerActorNr;
        return clone;
    }

    //for cube cloning while cube is in motion
    public void UpdateCubeClone(Cube original)
    {
        CubeClone.inRotation = original.inRotation;
        CubeClone.cubeRot = original.cubeRot;
    }

    public void PopulateList<T>(List<T> list, string prefabFolderPath)
    {
        if (list == null) list = new List<T>();
        UnityEngine.Object[] prefabs = Resources.LoadAll(prefabFolderPath);
        foreach (UnityEngine.Object prefab in prefabs)
        {
            T prefabGO = (T)(object)prefab;
            list.Add(prefabGO);
        }
    }

    public List<GameObject> ReturnListFromID(int id)
    {
        switch (id)
        {
            case 1:
                return WeaponPowerUps;
            case 2:
                return PowerUps;
        }
        return null;
    }

    private void FixedUpdate()
    {
        if (!started) CheckIfAllReady();
        if (!setScoreTable && !stopUpdateCalls) CheckIfAllSpawned();
    }

    private void CheckIfAllReady()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["LoadedIn"])
            {
                allReady = false;
            }
        }
        if (allReady)
        {
            started = true;
            PV.RPC("SyncStart_RPC", RpcTarget.AllBuffered);
        }
    }

    private void CheckIfAllSpawned()
    {
        allReady = true;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["ControllerInitialized"])
            {
                allReady = false;
            }
        }
        if (allReady)
        {
            InitializeGameInfo();
            stopUpdateCalls = true;
        }
    }

    private void InitializeScoreTable()
    {
        PV.RPC("InitializeMyScore_RPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        MultiplayerSettings.multiplayerSettings.SetCustomPlayerProperties("GameInfoSet", true);
    }

    public void StatChange(int actorNumber, string key)
    {
        PV.RPC("AddStat_RPC", RpcTarget.AllBuffered, actorNumber, key);
    }

    IEnumerator StartingMatch()
    {
        yield return new WaitForSeconds(5f);
        TimeStopped = false;
    }

    public void StopTime()
    {
        PV.RPC("StopTime_RPC", RpcTarget.AllBuffered);
    }

    public void StartTime()
    {
        PV.RPC("StartTime_RPC", RpcTarget.AllBuffered);
    }


    [PunRPC]
    private void InitializeMyScore_RPC(int actorNumber)
    {
        GameObject scoreObj = Instantiate(scorePrefab, transform);
        Score score = scoreObj.GetComponent<Score>();
        score.photonPlayer = PhotonView.Find(Int32.Parse(actorNumber + "001")).gameObject;
        if (!PhotonNetwork.CurrentRoom.GetPlayer(actorNumber).IsMasterClient) score.playerAvatar = PhotonView.Find(Int32.Parse(actorNumber + "002")).gameObject;
        else score.playerAvatar = PhotonView.Find(Int32.Parse(actorNumber + "003")).gameObject;
        score.actorNumber = actorNumber;
        scoreTable.Add(actorNumber, score);
    }

    [PunRPC]
    private void AddStat_RPC(int actorNumber, string key)
    {
        Score score = (Score)scoreTable[actorNumber];
        score.AddToStat(key);
    }

    [PunRPC]
    private void SyncStart_RPC()
    {
        if (GameManager.Manager.PV.IsMine) GameManager.Manager.InitalizeGameManager();
        StartCoroutine(StartingMatch());
    }

    [PunRPC]
    private void StopTime_RPC()
    {
        TimeStopped = true;
    }

    [PunRPC]
    private void StartTime_RPC()
    {
        TimeStopped = false;
    }

}

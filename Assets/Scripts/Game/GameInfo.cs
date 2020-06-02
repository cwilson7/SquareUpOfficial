using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class GameInfo : MonoBehaviour
{
    public static GameInfo GI;
    public PhotonView PV;
    public Hashtable scoreTable;
    public bool TimeStopped;

    private bool starting;

    [SerializeField] private GameObject scorePrefab;

    // Start is called before the first frame update
    void Awake()
    {
        GI = this;
        PV = GetComponent<PhotonView>();
        TimeStopped = true;
        starting = false;
    }

    private void Start()
    {
        InitializeScoreTable();
        PV.RPC("SyncStart_RPC", RpcTarget.AllBuffered);
    }

    private void InitializeScoreTable()
    {
        scoreTable = new Hashtable();
        PV.RPC("InitializeMyScore_RPC", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
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
        score.playerAvatar = PhotonView.Find(Int32.Parse(actorNumber + "002")).gameObject;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameInfo : MonoBehaviour
{
    public static GameInfo GI;
    public PhotonView PV;
    public Hashtable scoreTable;

    [SerializeField] private GameObject scorePrefab;
    
    // Start is called before the first frame update
    void Awake()
    {
        GI = this;
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InitializeScoreTable();
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

    [PunRPC]
    private void InitializeMyScore_RPC(int actorNumber)
    {
        GameObject scoreObj = Instantiate(scorePrefab, transform);
        Score score = scoreObj.GetComponent<Score>();
        score.actorNumber = actorNumber;
        scoreTable.Add(actorNumber, score);
    }

    [PunRPC]
    private void AddStat_RPC(int actorNumber, string key)
    {
        Score score = (Score)scoreTable[actorNumber];
        score.AddToStat(key);
    }

}

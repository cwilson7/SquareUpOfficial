using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using CustomUtilities;
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
    public GameObject FistContainer;
    public float startDelaySeconds = 3f;
    public Dictionary<int, GameObject> avatarClones;
    public int bestActorNr = -1;

    private bool started = false, setScoreTable = false, stopUpdateCalls = false;

    [SerializeField] private GameObject scorePrefab;

    // Start is called before the first frame update
    void Awake()
    {
        GI = this;
        scoreTable = new Hashtable();
        PV = GetComponent<PhotonView>();
    }



    public void InitializeGameInfo()
    {
        TimeStopped = true;
        started= false;
        CubeClone = CopyCube(Cube.cb);
        cubeCloned = true;
        Utils.PopulateList<GameObject>(WeaponPowerUps, "PhotonPrefabs/PowerUps/WeaponPowerUps");
        Utils.PopulateList<GameObject>(PowerUps, "PhotonPrefabs/PowerUps/OtherPowerUps");
        //Utils.PopulateList<RuntimeAnimatorController>(AnimatorControllers, "PhotonPrefabs/AnimatorControllers");
        InitializeScoreTable();
    }

    public Dictionary<int, GameObject> CopyAvatarsToList()
    {
        Dictionary<int, GameObject> dict = new Dictionary<int, GameObject>();
        foreach (Score score in scoreTable.Values)
        {
            GameObject avatar = score.playerAvatar.GetComponentInChildren<AvatarCharacteristics>().gameObject;
            GameObject avatarClone = Instantiate(avatar, new Vector3(0f, 0f, 0f), Quaternion.identity);
            avatarClone.SetActive(false);
            dict.Add(score.actorNumber, avatarClone);
        }
        return dict;
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
                break;
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

    public void StatChange(int actorNumber, Stat key)
    {
        string data = Enum.GetName(typeof(Stat), key);
        PV.RPC("AddStat_RPC", RpcTarget.AllBuffered, actorNumber, data);
    }

    IEnumerator StartingMatch()
    {
        yield return new WaitForSeconds(startDelaySeconds);
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

    public int WinningActorNumber()
    {
        int bestActor = -1, highestKills = -1;
        foreach (KeyValuePair<int, Player> kvp in PhotonNetwork.CurrentRoom.Players)
        {
            Score myStats = (Score)scoreTable[kvp.Key];
            if (myStats.playerStatistics[Stat.kills] > highestKills)
            {
                highestKills = myStats.playerStatistics[Stat.kills];
                bestActor = kvp.Key;
            }
        }
        return bestActor;
    }

    public void CheckWinner(int actorNumber)
    {
        Score contender = (Score)scoreTable[actorNumber];
        if (!scoreTable.ContainsKey(bestActorNr) || contender.playerStatistics[Stat.kills] > ((Score)scoreTable[bestActorNr]).playerStatistics[Stat.kills])
        {
            PV.RPC("ChangeWinner_RPC", RpcTarget.All, actorNumber, bestActorNr);
        }
    }

    //every time a kill sent out, check to see if there is a new winner
    //if there is not one then return
    //else call "Winning(bool isWinning)" on old winner and new winner
    //Winning(bool) does winner stuff
    
    [PunRPC]
    public void ChangeWinner_RPC(int newWinner, int oldWinner)
    {
        GI.bestActorNr = newWinner;

        Score newChamp = (Score)scoreTable[newWinner];
        newChamp.playerAvatar.GetComponent<Controller>().Winning(true);
        
        if (scoreTable.ContainsKey(oldWinner))
        {
            Score oldChamp = (Score)scoreTable[oldWinner];
            oldChamp.playerAvatar.GetComponent<Controller>().Winning(false);
        }
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
        Stat content = (Stat)Enum.Parse(typeof(Stat), key);
        score.AddToStat(content);
        if (content == Stat.kills && PV.IsMine) CheckWinner(actorNumber); 
    }


    [PunRPC]
    private void SyncStart_RPC()
    {
        if (GameManager.Manager.PV.IsMine)
        {
            GameManager.Manager.InitalizeGameManager();
            GameManager.Manager.StartCoroutine(GameManager.Manager.StartDelay());
        }

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
